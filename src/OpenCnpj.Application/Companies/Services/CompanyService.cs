using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using OpenCnpj.Application.Addresses.Domain;
using OpenCnpj.Application.Addresses.Repositories;
using OpenCnpj.Application.Addresses.Services;
using OpenCnpj.Application.AddressTypes.Domain;
using OpenCnpj.Application.AddressTypes.Repositories;
using OpenCnpj.Application.Cities.Domain;
using OpenCnpj.Application.Cities.Repositories;
using OpenCnpj.Application.Companies.Domain;
using OpenCnpj.Application.Companies.Models;
using OpenCnpj.Application.Companies.Repositories;
using OpenCnpj.Application.CompaniesSecondaryEconomicActivities.Domain;
using OpenCnpj.Application.CompaniesSecondaryEconomicActivities.Repositories;
using OpenCnpj.Application.CompanyContacts.Domain;
using OpenCnpj.Application.CompanyContacts.Repositories;
using OpenCnpj.Application.CompanySpecialSituations.Domain;
using OpenCnpj.Application.CompanySpecialSituations.Repositories;
using OpenCnpj.Application.Contacts.Domain;
using OpenCnpj.Application.Contacts.Repositories;
using OpenCnpj.Application.Countries.Domain;
using OpenCnpj.Application.Countries.Repositories;
using OpenCnpj.Application.EconomicActivities.Domain;
using OpenCnpj.Application.EconomicActivities.Repositories;
using OpenCnpj.Application.Enums;
using OpenCnpj.Application.LegalNatures.Domain;
using OpenCnpj.Application.LegalNatures.Repositories;
using OpenCnpj.Application.LegalRepresentatives.Domain;
using OpenCnpj.Application.LegalRepresentatives.Repositories;
using OpenCnpj.Application.Meis.Domain;
using OpenCnpj.Application.Meis.Repositories;
using OpenCnpj.Application.Partners.Domain;
using OpenCnpj.Application.Partners.Repositories;
using OpenCnpj.Application.PartnersQualifications.Domain;
using OpenCnpj.Application.PartnersQualifications.Repositories;
using OpenCnpj.Application.Phones.Domain;
using OpenCnpj.Application.Phones.Repositories;
using OpenCnpj.Application.RawRecords;
using OpenCnpj.Application.Simples.Domain;
using OpenCnpj.Application.Simples.Repositories;
using OpenCnpj.Application.SpecialSituations.Domain;
using OpenCnpj.Application.SpecialSituations.Repositories;
using OpenCnpj.Core.Configurations;
using OpenCnpj.Core.Database.Factory.Interfaces;
using OpenCnpj.Core.Extensions;
using OpenCnpj.Core.Helpers;
using Serilog;
using System.Collections.Concurrent;

namespace OpenCnpj.Application.Companies.Services;
public class CompanyService(IMongoDatabaseFactory mongoDatabaseFactory, IServiceProvider serviceProvider, TweakSettings tweakSettings, ILogger logger) : ICompanyService
{
    private readonly ConcurrentHashSet<(long CompanyId, long EconomicActivityId)> _secondaryActivityKeys = new();
    private readonly ConcurrentDictionary<string, LegalNature?> _legalNatureCache = new();
    private readonly ConcurrentDictionary<long, PartnerQualification?> _partnerQualificationCache = new();
    private readonly ConcurrentDictionary<string, Country?> _countryCache = new();
    private readonly ConcurrentDictionary<string, EconomicActivity?> _economicActivityCache = new();
    private readonly ConcurrentDictionary<string, SpecialSituation?> _specialSituationCache = new();
    private readonly ConcurrentDictionary<long, City?> _cityCache = new();
    private readonly ConcurrentDictionary<string, AddressType?> _addressTypeCache = new();
    private readonly ConcurrentDictionary<string, Company> _companiesCache = new();
    private readonly IdGenerator _addressTypeIdGen = new();
    private readonly IdGenerator _addressIdGen = new();
    private readonly IdGenerator _companyIdGen = new();
    private readonly IdGenerator _specialSituationIdGen = new();
    private readonly IdGenerator _companySpecialSituationIdGen = new();
    private readonly IdGenerator _partnerIdGen = new();
    private readonly IdGenerator _legalRepresentativeIdGen = new();
    private readonly IdGenerator _simpleIdGen = new();
    private readonly IdGenerator _meiIdGen = new();
    private readonly IdGenerator _phoneIdGen = new();
    private readonly IdGenerator _contactIdGen = new();

    private readonly ConcurrentBag<Company> _companiesToInsert = [];
    private readonly ConcurrentBag<AddressType> _addressTypesToInsert = [];
    private readonly ConcurrentBag<Address> _addressessToInsert = [];
    private readonly ConcurrentBag<SpecialSituation> _specialSituationsToInsert = [];
    private readonly ConcurrentBag<CompanySpecialSituation> _companySpecialSituationToInsert = [];
    private readonly ConcurrentBag<Partner> _partnersToInsert = [];
    private readonly ConcurrentBag<LegalRepresentative> _legalRepresentativesToInsert = [];
    private readonly ConcurrentBag<Simple> _simplesToInsert = [];
    private readonly ConcurrentBag<Mei> _meisToInsert = [];
    private readonly ConcurrentBag<Phone> _phonesToInsert = [];
    private readonly ConcurrentBag<Contact> _contactsToInsert = [];
    private readonly ConcurrentBag<CompanyContact> _companyContactsToInsert = [];
    private readonly ConcurrentBag<CompanySecondaryEconomicActivity> _companySecondaryEconomicActivityToInsert = [];

    private readonly ILogger _logger = logger.ForContext<CompanyService>();
    public async Task<Result> CreateCompanies(CancellationToken cancellationToken)
    {
        try
        {
            var companiesCollection = mongoDatabaseFactory.Database.GetCollection<CompanyRawRecord>("CompaniesRaw");
            int pageSize = tweakSettings.FormatRawDataSettings.RecordsBatchAmount;
            var page = 0;

            await PreloadCaches(cancellationToken);

            var semaphoreSlim = new SemaphoreSlim(tweakSettings.FormatRawDataSettings.AmountAtTheSameTime);
            var allPages = new List<Task<List<object>>>(); // cada página vai retornar coleções para inserir

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
                        (CompanyDetailedInformation c) => c.Establishments
                    ).Lookup(
                        mongoDatabaseFactory.Database.GetCollection<PartnerRawRecord>("PartnersRaw"),
                        c => c.BasicCnpj,
                        p => p.BasicCnpj,
                        (CompanyDetailedInformation c) => c.Partners
                    )
                    .Lookup(
                        mongoDatabaseFactory.Database.GetCollection<SimpleDataRawRecord>("SimplesDataRaw"),
                        c => c.BasicCnpj,
                        s => s.BasicCnpj,
                        (CompanyDetailedInformation c) => c.SimpleData
                    );

                using var cursor = await pipeline.ToCursorAsync(cancellationToken);
                var anyInPage = false;

                var companyBatch = new List<Task>();
                while (await cursor.MoveNextAsync(cancellationToken))
                {
                    foreach (var companyWithEstablishments in cursor.Current)
                    {
                        anyInPage = true;
                        await semaphoreSlim.WaitAsync(cancellationToken);

                        var task = Task.Run(async () =>
                        {
                            try
                            {
                                await ProcessCompany(companyWithEstablishments, cancellationToken);
                            }
                            finally
                            {
                                semaphoreSlim.Release();
                            }
                        }, cancellationToken);

                        companyBatch.Add(task);
                    }
                }

                if (!anyInPage)
                    break;

                await Task.WhenAll(companyBatch);

                using var scope = serviceProvider.CreateAsyncScope();
                var dbFactory = scope.ServiceProvider.GetRequiredService<IDatabaseFactory>();
                var services = ResolveServices(scope.ServiceProvider);

                //await dbFactory.BeginAsync();

                if (_addressTypesToInsert.Count > 0)
                {
                    await services.AddressTypeRepository.CopyToTable(_addressTypesToInsert, cancellationToken);
                    _addressTypesToInsert.Clear();
                }

                if (_addressessToInsert.Count > 0)
                {
                    await services.AddressRepository.CopyToTable(_addressessToInsert, cancellationToken);
                    _addressessToInsert.Clear();
                }

                if (_specialSituationsToInsert.Count > 0)
                {
                    await services.SpecialSituationRepository.CopyToTable(_specialSituationsToInsert, cancellationToken);
                    _specialSituationsToInsert.Clear();
                }

                if (_companiesToInsert.Count > 0)
                {
                    await services.CompanyRepository.CopyToTable(_companiesToInsert, cancellationToken);
                    _companiesToInsert.Clear();
                }

                if (_companySpecialSituationToInsert.Count > 0)
                {
                    await services.CompanySpecialSituationRepository.CopyToTable(_companySpecialSituationToInsert, cancellationToken);
                    _companySpecialSituationToInsert.Clear();
                }

                if (_phonesToInsert.Count > 0)
                {
                    await services.PhoneRepository.CopyToTable(_phonesToInsert, cancellationToken);
                    _phonesToInsert.Clear();
                }

                if (_contactsToInsert.Count > 0)
                {
                    await services.ContactRepository.CopyToTable(_contactsToInsert, cancellationToken);
                    _contactsToInsert.Clear();
                }

                if (_companyContactsToInsert.Count > 0)
                {
                    await services.CompanyContactRepository.CopyToTable(_companyContactsToInsert, cancellationToken);
                    _companyContactsToInsert.Clear();
                }

                if (_legalRepresentativesToInsert.Count > 0)
                {
                    await services.LegalRepresentativeRepository.CopyToTable(_legalRepresentativesToInsert, cancellationToken);
                    _legalRepresentativesToInsert.Clear();
                }

                if (_partnersToInsert.Count > 0)
                {
                    await services.PartnerRepository.CopyToTable(_partnersToInsert, cancellationToken);
                    _partnersToInsert.Clear();
                }

                if (_meisToInsert.Count > 0)
                {
                    await services.MeiRepository.CopyToTable(_meisToInsert, cancellationToken);
                    _meisToInsert.Clear();
                }

                if (_simplesToInsert.Count > 0)
                {
                    await services.SimpleRepository.CopyToTable(_simplesToInsert, cancellationToken);
                    _simplesToInsert.Clear();
                }

                if (_companySecondaryEconomicActivityToInsert.Count > 0)
                {
                    await services.CompanySecondaryEconomicActivityRepository.CopyToTable(_companySecondaryEconomicActivityToInsert, cancellationToken);
                    _companySecondaryEconomicActivityToInsert.Clear();
                }

                //await dbFactory.CommitAsync();

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

    private async Task ProcessCompany(CompanyDetailedInformation companyDetailedInformation, CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateAsyncScope();

        var services = ResolveServices(scope.ServiceProvider);

        foreach (var establishmentRawRecord in companyDetailedInformation.Establishments)
            await ProcessEstablishments(companyDetailedInformation, establishmentRawRecord, services, cancellationToken);

        foreach (var partnerRawRecord in companyDetailedInformation.Partners)
            await ProcessPartners(partnerRawRecord, services, cancellationToken);

        foreach (var simpleDataRawRecord in companyDetailedInformation.SimpleData)
            ProcessSimples(simpleDataRawRecord, cancellationToken);
    }

    private async Task ProcessEstablishments(CompanyRawRecord companyRawRecord, EstablishmentRawRecord establishmentRawRecord, ServiceResolvers services, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        var addressType = _addressTypeCache.GetOrAdd(establishmentRawRecord.Address.StreetType, CreateAddressType);

        var addressCreateResult = CreateAddress(addressType!.ID, establishmentRawRecord);
        if (addressCreateResult.IsFailure)
        {
            _logger.Warning(addressCreateResult.Error);
            return;
        }

        var legalNature = _legalNatureCache.GetValueOrDefault(companyRawRecord.LegalNatureCode.ToString());
        if (legalNature == null)
        {
            _logger.Warning("Unable to fetch the legal nature of the company.");
            return;
        }

        var mainPartnerQualification = _partnerQualificationCache.GetValueOrDefault(companyRawRecord.ResponsibleQualification);

        var country = await _countryCache.GetOrAddAsync(establishmentRawRecord.CountryCode,
            async c => await services.CountryRepository.GetByCode(c, cancellationToken));

        long companyID = _companyIdGen.NextId();

        if (!string.IsNullOrEmpty(establishmentRawRecord.SpecialStatus))
        {
            var specialSituation = _specialSituationCache.GetOrAdd(establishmentRawRecord.SpecialStatus, CreateSpecialSituation);

            CreateCompanySpecialSituation(specialSituation!.ID, companyID, establishmentRawRecord.SpecialStatusDate!.Value);
        }

        var mainEconomicActivity = _economicActivityCache.GetValueOrDefault(establishmentRawRecord.MainCnae);
        if (mainEconomicActivity == null)
        {
            _logger.Warning("Unable to fetch the economic activity of the company.");
            return;
        }

        Phone? phoneOne = null;
        if (int.TryParse(establishmentRawRecord.Contact.PhoneAreaCode1, out int phoneAreaCode1) &&
            int.TryParse(establishmentRawRecord.Contact.PhoneNumber1, out int phoneNumber1))
            phoneOne = Phone.Create(_phoneIdGen.NextId(), phoneAreaCode1, phoneNumber1);

        Phone? phoneTwo = null;
        if (int.TryParse(establishmentRawRecord.Contact.PhoneAreaCode2, out int phoneAreaCode2) &&
            int.TryParse(establishmentRawRecord.Contact.PhoneNumber2, out int phoneNumber2))
            phoneTwo = Phone.Create(_phoneIdGen.NextId(), phoneAreaCode2, phoneNumber2);

        int? faxAreaCode = null;
        if (int.TryParse(establishmentRawRecord.Contact.FaxAreaCode, out var parsedFaxAreaCode))
            faxAreaCode = parsedFaxAreaCode;

        int? faxNumber = null;
        if (int.TryParse(establishmentRawRecord.Contact.FaxNumber, out int parsedFaxNumber))
            faxNumber = parsedFaxNumber;

        List<Phone> phones = [];
        List<Contact> contacts = [];
        if (phoneOne != null)
        {
            phones.Add(phoneOne);
            contacts.Add(Contact.Create(_contactIdGen.NextId(), phoneOne.ID, faxAreaCode, faxNumber, establishmentRawRecord.Contact.Email));
        }
        if (phoneTwo != null)
        {
            phones.Add(phoneTwo);
            contacts.Add(Contact.Create(_contactIdGen.NextId(), phoneTwo.ID, faxAreaCode, faxNumber, establishmentRawRecord.Contact.Email));
        }

        CreateCompanySecondaryEconomicActivities(companyID, establishmentRawRecord.SecondaryCnaes);

        List<CompanyContact> companyContacts = [];
        companyContacts.AddRange(contacts.Select(c => CompanyContact.Create(companyID, c.ID)));

        string identifier = $"{establishmentRawRecord.BasicCnpj}{establishmentRawRecord.OrderCnpj}{establishmentRawRecord.CheckDigitCnpj}";

        var company = Company.Create(
            companyID,
            legalNature.ID,
            mainPartnerQualification?.ID,
            (int)companyRawRecord.CompanySize!,
            (int)establishmentRawRecord.HeadOfficeOrBranch,
            country?.ID,
            addressCreateResult.Value.ID,
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

        _phonesToInsert.AddRange(phones);
        _contactsToInsert.AddRange(contacts);
        _companyContactsToInsert.AddRange(companyContacts);

        _companiesToInsert.Add(company);
        _companiesCache[company.Identifier] = company;
    }

    private async Task ProcessPartners(PartnerRawRecord partnerRawRecord, ServiceResolvers services, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        var company = _companiesCache
            .First(c => c.Key.StartsWith(partnerRawRecord.BasicCnpj));

        if (!long.TryParse(partnerRawRecord.RepresentativeQualification, out long representativeQualificationCode))
        {
            _logger.Error("Unable to parse representative qualification.");
            return;
        }

        var legalRepresentativeQualification = await _partnerQualificationCache.GetOrAddAsync(representativeQualificationCode,
            async p => await services.PartnerQualificationRepository.GetByCode(representativeQualificationCode, cancellationToken));
        if (legalRepresentativeQualification == null)
        {
            _logger.Error("Unable to retreive representative qualification.");
            return;
        }

        var legalRepresentative = LegalRepresentative.Create(_legalRepresentativeIdGen.NextId(), legalRepresentativeQualification.ID, partnerRawRecord.RepresentativeDocument, partnerRawRecord.RepresentativeName);
        if (!long.TryParse(partnerRawRecord.PartnerQualification, out long partnerQualificationCode))
        {
            _logger.Error("Unable to parse representative qualification.");
            return;
        }

        var partnerQualification = await _partnerQualificationCache.GetOrAddAsync(partnerQualificationCode,
            async p => await services.PartnerQualificationRepository.GetByCode(partnerQualificationCode, cancellationToken));
        if (partnerQualification == null)
        {
            _logger.Error("Unable to retreive partner qualification.");
            return;
        }

        var country = await _countryCache.GetOrAddAsync(partnerRawRecord.CountryCode,
            async c => await services.CountryRepository.GetByCode(c, cancellationToken));
        long? countryID = country?.ID;

        if (!Enum.TryParse(partnerRawRecord.AgeRange, out EAgeRanges ageRange))
        {
            _logger.Error("Unable to retreive partner age range.");
            return;
        }

        var partner = Partner.Create(_partnerIdGen.NextId(), company.Value.ID, (int)partnerRawRecord.PartnerType, legalRepresentative.ID, partnerQualification.ID, partnerRawRecord.PartnerName, partnerRawRecord.PartnerDocument, partnerRawRecord.EntryDate!.Value, countryID, (int)ageRange);

        _legalRepresentativesToInsert.Add(legalRepresentative);
        _partnersToInsert.Add(partner);

        return;
    }

    private void ProcessSimples(SimpleDataRawRecord simpleDataRawRecord, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        var company = _companiesCache
            .First(c => c.Key.StartsWith(simpleDataRawRecord.BasicCnpj));

        Mei? mei = null;
        if (simpleDataRawRecord.OptInMei.HasValue && simpleDataRawRecord.OptInMei.Value)
            mei = Mei.Create(_meiIdGen.NextId(), simpleDataRawRecord.MeiOptionDate, simpleDataRawRecord.MeiExclusionDate);

        var simple = Simple.Create(_simpleIdGen.NextId(), company.Value.ID, mei?.ID, simpleDataRawRecord.OptInSimple, simpleDataRawRecord.SimpleOptionDate, simpleDataRawRecord.SimpleExclusionDate);

        if (mei != null)
            _meisToInsert.Add(mei);

        _simplesToInsert.Add(simple);
    }

    private AddressType CreateAddressType(string streetType)
    {
        var addressType = AddressType.Create(_addressTypeIdGen.NextId(), streetType);

        _addressTypesToInsert.Add(addressType);

        return addressType;
    }

    private Result<Address> CreateAddress(long addressTypeID, EstablishmentRawRecord establishmentRawRecord)
    {
        if (!long.TryParse(establishmentRawRecord.Address.MunicipalityCode, out var cityCode))
            return Result.Failure<Address>("Unable to parse MunicipalityCode");

        var city = _cityCache.GetValueOrDefault(cityCode);
        if (city == null)
            return Result.Failure<Address>("The city of the establishment could not be retreived.");

        int? zipCode = null;
        if (int.TryParse(establishmentRawRecord.Address.ZipCode, out var parsedZipCode))
            zipCode = parsedZipCode;

        int? addressNumber = null;
        if (int.TryParse(establishmentRawRecord.Address.Number, out var parsedAddressNumber))
            addressNumber = parsedAddressNumber;

        var address = Address.Create(_addressIdGen.NextId(), addressTypeID, city.ID, establishmentRawRecord.Address.StreetName, zipCode, establishmentRawRecord.Address.AdditionalAddressInfo, establishmentRawRecord.Address.District, addressNumber, establishmentRawRecord.Address.State);

        _addressessToInsert.Add(address);

        return address;
    }

    private SpecialSituation CreateSpecialSituation(string specialSituationDescription)
    {
        var specialSituation = SpecialSituation.Create(_specialSituationIdGen.NextId(), specialSituationDescription);

        _specialSituationsToInsert.Add(specialSituation);

        return specialSituation;
    }

    private void CreateCompanySpecialSituation(long specialSituationID, long companyID, DateTime specialStatusDate)
    {
        var companySpecialSituation = CompanySpecialSituation.Create(_companySpecialSituationIdGen.NextId(), companyID, specialSituationID, specialStatusDate);

        _companySpecialSituationToInsert.Add(companySpecialSituation);
    }

    private void CreateCompanySecondaryEconomicActivities(long companyID, string secondaryCnaes)
    {
        var economicActivitiesCodes = secondaryCnaes.Split(',');
        foreach (var economicActivityCode in economicActivitiesCodes)
        {
            var economicActivity = _economicActivityCache.GetValueOrDefault(economicActivityCode);
            if (economicActivity == null)
                continue;

            var key = (companyID, economicActivity.ID);

            if (_secondaryActivityKeys.Add(key))
            {
                var companySecondaryEconomicActivity = CompanySecondaryEconomicActivity.Create(companyID, economicActivity.ID);
                _companySecondaryEconomicActivityToInsert.Add(companySecondaryEconomicActivity);
            }
        }
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
            _addressTypeIdGen.SetIfGreater(addressType.ID);
        }
    }

    private async Task PreloadSpecialSituations(ISpecialSituationRepository repository, CancellationToken cancellationToken)
    {
        var specialSituations = await repository.GetAll(cancellationToken);
        foreach (var specialSituation in specialSituations)
        {
            _specialSituationCache[specialSituation.Description] = specialSituation;
            _specialSituationIdGen.SetIfGreater(specialSituation.ID);
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
            AddressRepository = serviceProvider.GetRequiredService<IAddressRepository>(),
            LegalRepresentativeRepository = serviceProvider.GetRequiredService<ILegalRepresentativeRepository>(),
            PartnerRepository = serviceProvider.GetRequiredService<IPartnerRepository>(),
            MeiRepository = serviceProvider.GetRequiredService<IMeiRepository>(),
            SimpleRepository = serviceProvider.GetRequiredService<ISimpleRepository>(),
            PhoneRepository = serviceProvider.GetRequiredService<IPhoneRepository>(),
            ContactRepository = serviceProvider.GetRequiredService<IContactRepository>(),
            CompanyContactRepository = serviceProvider.GetRequiredService<ICompanyContactRepository>(),
            CompanySecondaryEconomicActivityRepository = serviceProvider.GetRequiredService<ICompanySecondaryEconomicActivityRepository>()
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
        public ILegalRepresentativeRepository LegalRepresentativeRepository { get; set; } = null!;
        public IPartnerRepository PartnerRepository { get; set; } = null!;
        public IMeiRepository MeiRepository { get; set; } = null!;
        public ISimpleRepository SimpleRepository { get; set; } = null!;
        public IPhoneRepository PhoneRepository { get; set; } = null!;
        public IContactRepository ContactRepository { get; set; } = null!;
        public ICompanyContactRepository CompanyContactRepository { get; set; } = null!;
        public ICompanySecondaryEconomicActivityRepository CompanySecondaryEconomicActivityRepository { get; set; } = null!;
    }
}