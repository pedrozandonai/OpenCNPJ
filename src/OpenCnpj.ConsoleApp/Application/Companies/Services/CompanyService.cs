using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using OpenCnpj.ConsoleApp.Application.Addresses.Services;
using OpenCnpj.ConsoleApp.Application.Companies.Domain;
using OpenCnpj.ConsoleApp.Application.Companies.Models;
using OpenCnpj.ConsoleApp.Application.Companies.Repositories;
using OpenCnpj.ConsoleApp.Application.CompanySpecialSituations.Domain;
using OpenCnpj.ConsoleApp.Application.CompanySpecialSituations.Repositories;
using OpenCnpj.ConsoleApp.Application.Countries.Domain;
using OpenCnpj.ConsoleApp.Application.Countries.Repositories;
using OpenCnpj.ConsoleApp.Application.EconomicActivities.Domain;
using OpenCnpj.ConsoleApp.Application.EconomicActivities.Repositories;
using OpenCnpj.ConsoleApp.Application.LegalNatures.Domain;
using OpenCnpj.ConsoleApp.Application.LegalNatures.Repositories;
using OpenCnpj.ConsoleApp.Application.PartnersQualifications.Domain;
using OpenCnpj.ConsoleApp.Application.PartnersQualifications.Repositories;
using OpenCnpj.ConsoleApp.Application.RawRecords;
using OpenCnpj.ConsoleApp.Application.SpecialSituations.Domain;
using OpenCnpj.ConsoleApp.Application.SpecialSituations.Repositories;
using OpenCnpj.ConsoleApp.Configurations;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;
using OpenCnpj.ConsoleApp.Extensions;
using Serilog;
using System.Collections.Concurrent;
using System.Threading;

namespace OpenCnpj.ConsoleApp.Application.Companies.Services;
public class CompanyService(IMongoDatabaseFactory mongoDatabaseFactory, IServiceProvider serviceProvider, TweakSettings tweakSettings, ILogger logger) : ICompanyService
{
    private readonly ConcurrentDictionary<string, LegalNature?> _legalNatureCache = new();
    private readonly ConcurrentDictionary<long, PartnerQualification?> _partnerQualificationCache = new();
    private readonly ConcurrentDictionary<string, Country?> _countryCache = new();
    private readonly ConcurrentDictionary<string, EconomicActivity?> _economicActivityCache = new();
    private readonly ConcurrentDictionary<string, SpecialSituation?> _specialSituationCache = new();

    private readonly ILogger _logger = logger.ForContext<CompanyService>();
    public async Task<Result> CreateCompanies(CancellationToken cancellationToken)
    {
        try
        {
            var companiesCollection = mongoDatabaseFactory.Database.GetCollection<CompanyRawRecord>("CompaniesRaw");
            int pageSize = tweakSettings.FormatRawDataSettings.RecordsBatchAmount;
            var page = 0;
            int maxParallelCompanies = tweakSettings.FormatRawDataSettings.AmountAtTheSameTime;
            var companySemaphore = new SemaphoreSlim(maxParallelCompanies);

            await PreloadCaches(cancellationToken);

            while (true)
            {
                _logger.Information("Current page: {0}", page);

                var pipeline = companiesCollection.Aggregate()
                    .Skip(page * pageSize)
                    .Limit(pageSize)
                    .Lookup(
                        mongoDatabaseFactory.Database.GetCollection<EstablishmentRawRecord>("EstablishmentsRaw"),
                        c => c.BasicCnpj,
                        e => e.BasicCnpj,
                        (CompanyWithEstablishments c) => c.Establishments
                    );

                using var cursor = await pipeline.ToCursorAsync(cancellationToken);
                var anyInPage = false;
                var companyTasks = new List<Task>();

                while (await cursor.MoveNextAsync(cancellationToken))
                {
                    foreach (var companyWithEstablishments in cursor.Current)
                    {
                        anyInPage = true;

                        await companySemaphore.WaitAsync(cancellationToken);

                        companyTasks.Add(ProcessCompany(companyWithEstablishments, companySemaphore, cancellationToken));
                    }
                }

                await Task.WhenAll(companyTasks);

                if (!anyInPage) break;
                page++;
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An exception occured while trying to create 'Company' domain.");
            return Result.Failure("An error occured while trying to create 'Company' domain.");
        }
    }

    private async Task ProcessCompany(CompanyWithEstablishments companyWithEstablishments, SemaphoreSlim semaphore, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = serviceProvider.CreateAsyncScope();
            var dbFactory = scope.ServiceProvider.GetRequiredService<IDatabaseFactory>();

            var services = ResolveServices(scope.ServiceProvider);

            var companiesToInsert = new List<Company?>();

            await dbFactory.BeginAsync();

            foreach (var establishmentRawRecord in companyWithEstablishments.Establishments)
                companiesToInsert.Add(await ProcessEstablishments(companyWithEstablishments, establishmentRawRecord, services, cancellationToken));

            if (companiesToInsert.Count != 0)
                await services.CompanyRepository.Insert(companiesToInsert!, cancellationToken);

            await dbFactory.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An error when creating company.");
            try
            {
                using var scope = serviceProvider.CreateAsyncScope();
                var dbFactory = scope.ServiceProvider.GetRequiredService<IDatabaseFactory>();
                if (dbFactory != null)
                    await dbFactory.RollbackAsync();
            }
            catch { /* swallow */ }
        }
        finally
        {
            semaphore.Release();
        }
    }

    private async Task<Company?> ProcessEstablishments(CompanyRawRecord companyRawRecord, EstablishmentRawRecord establishmentRawRecord, ServiceResolvers services, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return null;

        var address = await services.AddressService.CreateAddressByEstablishmentRawRecord(establishmentRawRecord, cancellationToken);
        if (address.IsFailure)
        {
            _logger.Warning(address.Error);
            return null;
        }

        var legalNature = await _legalNatureCache.GetOrAddAsync(companyRawRecord.LegalNatureCode.ToString(), async c =>
            await services.LegalNatureRepository.GetByCode(c, cancellationToken));
        if (legalNature == null)
        {
            _logger.Warning("Unable to fetch the legal nature of the company.");
            return null;
        }

        var mainPartnerQualification = await _partnerQualificationCache.GetOrAddAsync(companyRawRecord.ResponsibleQualification,
            async p => await services.PartnerQualificationRepository.GetByCode(companyRawRecord.ResponsibleQualification, cancellationToken));

        var country = await _countryCache.GetOrAddAsync(establishmentRawRecord.CountryCode,
            async c => await services.CountryRepository.GetByCode(c, cancellationToken));

        int? companySpecialSituationID = null;
        if (!string.IsNullOrEmpty(establishmentRawRecord.SpecialStatus))
        {
            var companySpecialSituationResult = await CreateAndRetreiveCompanySpecialSituationID(establishmentRawRecord.SpecialStatus, establishmentRawRecord.SpecialStatusDate!.Value, services, cancellationToken);
            if (companySpecialSituationResult.IsFailure)
                return null;

            companySpecialSituationID = companySpecialSituationResult.Value;
        }

        var mainEconomicActivity = await _economicActivityCache.GetOrAddAsync(establishmentRawRecord.MainCnae, async e => await services.EconomicActivityRepository.GetByCode(e, cancellationToken));
        if (mainEconomicActivity == null)
        {
            _logger.Warning("Unable to fetch the economic activity of the company.");
            return null;
        }

        string identifier = $"{establishmentRawRecord.BasicCnpj}{establishmentRawRecord.OrderCnpj}{establishmentRawRecord.CheckDigitCnpj}";

        var company = Company.Create(
            legalNature.ID,
            mainPartnerQualification?.ID,
            (int)companyRawRecord.CompanySize!,
            (int)establishmentRawRecord.HeadOfficeOrBranch,
            country?.ID,
            address.Value.ID,
            companySpecialSituationID,
            mainEconomicActivity.ID,
            identifier,
            companyRawRecord.CorporateName,
            companyRawRecord.ShareCapital,
            companyRawRecord.ResponsibleFederativeEntity,
            establishmentRawRecord.TradeName,
            establishmentRawRecord.RegistrationStatusDate,
            establishmentRawRecord.ForeignCityName,
            establishmentRawRecord.StartActivityDate!.Value
        );

        return company;
    }

    private async Task<Result<int>> CreateAndRetreiveCompanySpecialSituationID(string specialStatusDescription, DateTime specialStatusDate, ServiceResolvers services, CancellationToken cancellationToken)
    {
        var specialSituation = await _specialSituationCache.GetOrAddAsync(specialStatusDescription,
            async s => await services.SpecialSituationRepository.GetByDescription(s, cancellationToken));

        if (specialSituation == null)
        {
            specialSituation = SpecialSituation.Create(specialStatusDescription);

            var specialSituationID = await services.SpecialSituationRepository.Insert(specialSituation, cancellationToken);

            var setIDResult = specialSituation.SetID(specialSituationID);
            if (setIDResult.IsFailure)
            {
                string errorMessage = "Unable to create a new special situation.";

                _logger.Warning(errorMessage);

                return Result.Failure<int>(errorMessage);
            }

            _specialSituationCache[specialStatusDescription] = specialSituation;
        }

        var companySpecialSituation = CompanySpecialSituation.Create(specialSituation.ID, specialStatusDate);

        return await services.CompanySpecialSituationRepository.Insert(companySpecialSituation, cancellationToken);
    }

    private async Task PreloadCaches(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = serviceProvider.CreateAsyncScope();
            var dbFactory = scope.ServiceProvider.GetRequiredService<IDatabaseFactory>();

            var services = ResolveServices(scope.ServiceProvider);

            await dbFactory.BeginAsync();

            await PreloadLegalNatures(services.LegalNatureRepository, cancellationToken);
            await PreloadEconomicActivities(services.EconomicActivityRepository, cancellationToken);
            await PreloadPartnerQualification(services.PartnerQualificationRepository, cancellationToken);
            await PreloadCountries(services.CountryRepository, cancellationToken);

            _logger.Information("Cache preloading completed. LegalNatures: {LN}, EconomicActivities: {EA}, Countries: {C}",
                _legalNatureCache.Count, _economicActivityCache.Count, _countryCache.Count);
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Failed to preload caches, will load on demand");
        }
    }

    private async Task PreloadLegalNatures(ILegalNatureRepository repository, CancellationToken cancellationToken)
    {
        try
        {
            var legalNatures = await repository.GetAll(cancellationToken);
            foreach(var legalNature in legalNatures)
                _legalNatureCache[legalNature.Code] = legalNature;
        }
        catch(Exception ex)
        {
            _logger.Warning(ex, "Unable to pre-load legal natures chache.");
        }
    }

    private async Task PreloadEconomicActivities(IEconomicActivityRepository repository, CancellationToken cancellationToken)
    {
        try
        {
            var economicActivities = await repository.GetAll(cancellationToken);
            foreach (var economicActivity in economicActivities)
                _economicActivityCache[economicActivity.Code] = economicActivity;
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Unable to pre-load economic activities chache.");
        }
    }

    private async Task PreloadPartnerQualification(IPartnerQualificationRepository repository, CancellationToken cancellationToken)
    {
        try
        {
            var partnerQualifications = await repository.GetAll(cancellationToken);
            foreach (var partnerQualification in partnerQualifications)
                _partnerQualificationCache[partnerQualification.Code] = partnerQualification;
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Unable to pre-load partner qualifications chache.");
        }
    }

    private async Task PreloadCountries(ICountryRepository repository, CancellationToken cancellationToken)
    {
        try
        {
            var countries = await repository.GetAll(cancellationToken);
            foreach (var country in countries)
                _countryCache[country.Code] = country;
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Unable to pre-load countries chache.");
        }
    }

    private static ServiceResolvers ResolveServices(IServiceProvider serviceProvider)
    {
        return new ServiceResolvers
        {
            AddressService = serviceProvider.GetRequiredService<IAddressService>(),
            LegalNatureRepository = serviceProvider.GetRequiredService<ILegalNatureRepository>(),
            PartnerQualificationRepository = serviceProvider.GetRequiredService<IPartnerQualificationRepository>(),
            CountryRepository = serviceProvider.GetRequiredService<ICountryRepository>(),
            SpecialSituationRepository = serviceProvider.GetRequiredService<ISpecialSituationRepository>(),
            EconomicActivityRepository = serviceProvider.GetRequiredService<IEconomicActivityRepository>(),
            CompanyRepository = serviceProvider.GetRequiredService<ICompanyRepository>(),
            CompanySpecialSituationRepository = serviceProvider.GetRequiredService<ICompanySpecialSituationRepository>()
        };
    }

    private sealed class ServiceResolvers
    {
        public IAddressService AddressService { get; set; } = null!;
        public ILegalNatureRepository LegalNatureRepository { get; set; } = null!;
        public IPartnerQualificationRepository PartnerQualificationRepository { get; set; } = null!;
        public ICountryRepository CountryRepository { get; set; } = null!;
        public ISpecialSituationRepository SpecialSituationRepository { get; set; } = null!;
        public IEconomicActivityRepository EconomicActivityRepository { get; set; } = null!;
        public ICompanyRepository CompanyRepository { get; set; } = null!;
        public ICompanySpecialSituationRepository CompanySpecialSituationRepository { get; set; } = null!;
    }
}