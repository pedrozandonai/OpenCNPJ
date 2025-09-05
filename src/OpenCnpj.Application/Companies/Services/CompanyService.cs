using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using OpenCnpj.Application.Addresses.Domain;
using OpenCnpj.Application.Addresses.Services;
using OpenCnpj.Application.AddressTypes.Domain;
using OpenCnpj.Application.Cities.Domain;
using OpenCnpj.Application.Companies.Domain;
using OpenCnpj.Application.Companies.Models;
using OpenCnpj.Application.CompanySpecialSituations.Domain;
using OpenCnpj.Application.Countries.Domain;
using OpenCnpj.Application.EconomicActivities.Domain;
using OpenCnpj.Application.LegalNatures.Domain;
using OpenCnpj.Application.PartnersQualifications.Domain;
using OpenCnpj.Application.RawRecords;
using OpenCnpj.Application.Reasons.Domain;
using OpenCnpj.Application.SpecialSituations.Domain;
using OpenCnpj.Core.Configurations;
using OpenCnpj.Core.Database;
using OpenCnpj.Core.Database.Factory.Interfaces;
using OpenCnpj.Core.Extensions;
using OpenCnpj.Core.Helpers;
using OpenCnpj.Core.Interfaces;
using Serilog;
using System.Collections.Concurrent;

namespace OpenCnpj.Application.Companies.Services;
public class CompanyService(IMongoDatabaseFactory mongoDatabaseFactory, IServiceProvider serviceProvider, TweakSettings tweakSettings, ILogger logger) : ICompanyService
{
    private readonly ConcurrentDictionary<string, LegalNature?> _legalNatureCache = new();
    private readonly ConcurrentDictionary<long, PartnerQualification?> _partnerQualificationCache = new();
    private readonly ConcurrentDictionary<string, Country?> _countryCache = new();
    private readonly ConcurrentDictionary<string, EconomicActivity?> _economicActivityCache = new();
    private readonly ConcurrentDictionary<string, SpecialSituation?> _specialSituationCache = new();
    private readonly ConcurrentDictionary<long, City?> _cityCache = new();
    private readonly ConcurrentDictionary<string, AddressType?> _addressTypeCache = new();
    private readonly IdGenerator _addressTypeIdGen = new();
    private readonly IdGenerator _addressIdGen = new();
    private readonly IdGenerator _companyIdGen = new();
    private readonly IdGenerator _specialSituationIdGen = new();
    private readonly IdGenerator _companySpecialSituationIdGen = new();

    private readonly ConcurrentBag<Company> _companiesToInsert = [];
    private readonly ConcurrentBag<AddressType> _addressTypesToInsert = [];
    private readonly ConcurrentBag<Address> _addressesToInsert = [];
    private readonly ConcurrentBag<SpecialSituation> _specialSituationsToInsert = [];
    private readonly ConcurrentBag<CompanySpecialSituation> _companySpecialSituationToInsert = [];

    private readonly ILogger _logger = logger.ForContext<CompanyService>();
    public async Task<Result> CreateCompanies(CancellationToken cancellationToken)
    {
        try
        {
            var companiesCollection = mongoDatabaseFactory.Database.GetCollection<CompanyRawRecord>("CompaniesRaw");
            int pageSize = tweakSettings.FormatRawDataSettings.RecordsBatchAmount;
            var page = 0;

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

                        companyTasks.Add(ProcessCompany(companyWithEstablishments, cancellationToken));
                    }
                }

                using var scope = serviceProvider.CreateAsyncScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
                var services = ResolveServices(scope.ServiceProvider);

                await Task.WhenAll(companyTasks);

                await dbContext.Database.BeginTransactionAsync(cancellationToken);

                if (_addressTypesToInsert.Count > 0)
                {
                    await services.AddressTypeRepository.Insert(_addressTypesToInsert, cancellationToken);

                    _addressTypesToInsert.Clear();
                }

                if (_addressesToInsert.Count > 0)
                {
                    await services.AddressRepository.Insert(_addressesToInsert, cancellationToken);

                    _addressesToInsert.Clear();
                }

                if (_specialSituationsToInsert.Count > 0)
                {
                    await services.SpecialSituationRepository.Insert(_specialSituationsToInsert, cancellationToken);

                    _specialSituationsToInsert.Clear();
                }

                if (_companySpecialSituationToInsert.Count > 0)
                {
                    await services.CompanySpecialSituationRepository.Insert(_companySpecialSituationToInsert, cancellationToken);

                    _companySpecialSituationToInsert.Clear();
                }

                if (_companiesToInsert.Count > 0)
                {
                    await services.CompanyRepository.Insert(_companiesToInsert, cancellationToken);

                    _companiesToInsert.Clear();
                }

                await services.CompanyRepository.SaveAllChanges(cancellationToken);

                await dbContext.Database.CommitTransactionAsync(cancellationToken);

                if (!anyInPage)
                    break;

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

    private async Task ProcessCompany(CompanyWithEstablishments companyWithEstablishments, CancellationToken cancellationToken)
    {

        using var scope = serviceProvider.CreateAsyncScope();

        var services = ResolveServices(scope.ServiceProvider);

        var tasks = companyWithEstablishments.Establishments.Select(async (establishmentRawRecord) =>
        {
            try
            {
                await ProcessEstablishments(companyWithEstablishments, establishmentRawRecord, services, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An exception has occurred while creating new companies by establishments.");
            }
        });

        await Task.WhenAll(tasks);
    }

    private async Task ProcessEstablishments(CompanyRawRecord companyRawRecord, EstablishmentRawRecord establishmentRawRecord, ServiceResolvers services, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        var addressType = _addressTypeCache.GetOrAdd(establishmentRawRecord.Address.StreetType, CreateAddressType);

        var addressCreateResult = await CreateAddress(services, addressType!, establishmentRawRecord, cancellationToken);
        if (addressCreateResult.IsFailure)
        {
            _logger.Warning(addressCreateResult.Error);
            return;
        }

        var legalNature = await _legalNatureCache.GetOrAddAsync(companyRawRecord.LegalNatureCode.ToString(), async c =>
            await services.LegalNatureRepository.GetByCode(c, cancellationToken));
        if (legalNature == null)
        {
            _logger.Warning("Unable to fetch the legal nature of the company.");
            return;
        }

        var mainPartnerQualification = await _partnerQualificationCache.GetOrAddAsync(companyRawRecord.ResponsibleQualification,
            async p => await services.PartnerQualificationRepository.GetByCode(companyRawRecord.ResponsibleQualification, cancellationToken));

        var country = await _countryCache.GetOrAddAsync(establishmentRawRecord.CountryCode,
            async c => await services.CountryRepository.GetByCode(c, cancellationToken));

        CompanySpecialSituation? companySpecialSituation = null;
        if (!string.IsNullOrEmpty(establishmentRawRecord.SpecialStatus))
        {
            var specialSituation = _specialSituationCache.GetOrAdd(establishmentRawRecord.SpecialStatus, CreateSpecialSituation);

             companySpecialSituation = CreateCompanySpecialSituation(specialSituation!, establishmentRawRecord.SpecialStatusDate!.Value);
        }

        var mainEconomicActivity = await _economicActivityCache.GetOrAddAsync(establishmentRawRecord.MainCnae, async e => await services.EconomicActivityRepository.GetByCode(e, cancellationToken));
        if (mainEconomicActivity == null)
        {
            _logger.Warning("Unable to fetch the economic activity of the company.");
            return;
        }

        string identifier = $"{establishmentRawRecord.BasicCnpj}{establishmentRawRecord.OrderCnpj}{establishmentRawRecord.CheckDigitCnpj}";

        var company = Company.Create(
            _companyIdGen.NextId(),
            legalNature,
            mainPartnerQualification,
            companyRawRecord.CompanySize!.Value,
            establishmentRawRecord.HeadOfficeOrBranch,
            country,
            addressCreateResult.Value,
            companySpecialSituation,
            mainEconomicActivity,
            identifier,
            companyRawRecord.CorporateName,
            companyRawRecord.ShareCapital,
            companyRawRecord.ResponsibleFederativeEntity,
            establishmentRawRecord.TradeName,
            establishmentRawRecord.RegistrationStatusDate,
            establishmentRawRecord.ForeignCityName,
            establishmentRawRecord.StartActivityDate!.Value
        );

        _companiesToInsert.Add(company);
    }

    private AddressType CreateAddressType(string streetType)
    {
        var addressType = AddressType.Create((int)_addressTypeIdGen.NextId(), streetType);

        _addressTypesToInsert.Add(addressType);

        return addressType;
    }

    private async Task<Result<Address>> CreateAddress(ServiceResolvers services, AddressType addressType, EstablishmentRawRecord establishmentRawRecord, CancellationToken cancellationToken)
    {
        if (!long.TryParse(establishmentRawRecord.Address.MunicipalityCode, out var cityCode))
            return Result.Failure<Address>("Unable to parse MunicipalityCode");

        var city = await _cityCache.GetOrAddAsync(cityCode, async c =>
            await services.CityRepository.GetByCode(c, cancellationToken));
        if (city == null)
            return Result.Failure<Address>("The city of the establishment could not be retreived.");

        int? zipCode = null;
        if (int.TryParse(establishmentRawRecord.Address.ZipCode, out var parsedZipCode))
            zipCode = parsedZipCode;

        int? addressNumber = null;
        if (int.TryParse(establishmentRawRecord.Address.Number, out var parsedAddressNumber))
            addressNumber = parsedAddressNumber;

        var address = Address.Create(_addressIdGen.NextId(), addressType, city, establishmentRawRecord.Address.StreetName, zipCode, establishmentRawRecord.Address.AdditionalAddressInfo, establishmentRawRecord.Address.District, addressNumber, establishmentRawRecord.Address.State);

        _addressesToInsert.Add(address);

        return address;
    }

    private SpecialSituation CreateSpecialSituation(string specialSituationDescription)
    {
        var specialSituation = SpecialSituation.Create((int)_specialSituationIdGen.NextId(), specialSituationDescription);

        _specialSituationsToInsert.Add(specialSituation);

        return specialSituation;
    }

    private CompanySpecialSituation CreateCompanySpecialSituation(SpecialSituation specialSituation, DateTime specialStatusDate)
    {
        var companySpecialSituation = CompanySpecialSituation.Create(_companySpecialSituationIdGen.NextId(), specialSituation, specialStatusDate);

        _companySpecialSituationToInsert.Add(companySpecialSituation);

        return companySpecialSituation;
    }

    private async Task PreloadCaches(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = serviceProvider.CreateAsyncScope();

            var services = ResolveServices(scope.ServiceProvider);

            await PreloadLegalNatures(services.LegalNatureRepository, cancellationToken);
            await PreloadEconomicActivities(services.EconomicActivityRepository, cancellationToken);
            await PreloadPartnerQualification(services.PartnerQualificationRepository, cancellationToken);
            await PreloadCountries(services.CountryRepository, cancellationToken);
            await PreloadCities(services.CityRepository, cancellationToken);
            await PreloadAddressTypes(services.AddressTypeRepository, cancellationToken);
            await PreloadSpecialSituations(services.SpecialSituationRepository, cancellationToken);

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
            foreach (var legalNature in legalNatures)
                _legalNatureCache[legalNature.Code] = legalNature;
        }
        catch (Exception ex)
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

    private async Task PreloadCities(ICityRepository repository, CancellationToken cancellationToken)
    {
        try
        {
            var cities = await repository.GetAll(cancellationToken);
            foreach (var city in cities)
                _cityCache[city.Code] = city;
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Unable to pre-load cities chache.");
        }
    }

    private async Task PreloadAddressTypes(IAddressTypeRepository repository, CancellationToken cancellationToken)
    {
        var addressTypes = await repository.GetAll(cancellationToken);
        foreach (var addressType in addressTypes)
        {
            _addressTypeCache[addressType.Description] = addressType;
            _addressTypeIdGen.SetIfGreater(addressType.Id);
        }
    }

    private async Task PreloadSpecialSituations(ISpecialSituationRepository repository, CancellationToken cancellationToken)
    {
        var specialSituations = await repository.GetAll(cancellationToken);
        foreach (var specialSituation in specialSituations)
        {
            _specialSituationCache[specialSituation.Description] = specialSituation;
            _specialSituationIdGen.SetIfGreater(specialSituation.Id);
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
            CompanySpecialSituationRepository = serviceProvider.GetRequiredService<ICompanySpecialSituationRepository>(),
            CityRepository = serviceProvider.GetRequiredService<ICityRepository>(),
            AddressTypeRepository = serviceProvider.GetRequiredService<IAddressTypeRepository>(),
            AddressRepository = serviceProvider.GetRequiredService<IAddressRepository>()
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
        public ICityRepository CityRepository { get; set; } = null!;
        public IAddressTypeRepository AddressTypeRepository { get; set; } = null!;
        public IAddressRepository AddressRepository { get; set; } = null!;
    }
}