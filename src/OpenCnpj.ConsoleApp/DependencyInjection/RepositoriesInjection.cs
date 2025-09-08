using Microsoft.Extensions.DependencyInjection;
using OpenCnpj.Application.Addresses.Repositories;
using OpenCnpj.Application.AddressTypes.Repositories;
using OpenCnpj.Application.Batches.Batches.Repositories;
using OpenCnpj.Application.Batches.BatchFiles.Repositories;
using OpenCnpj.Application.Cities.Repositories;
using OpenCnpj.Application.Companies.Repositories;
using OpenCnpj.Application.CompaniesSecondaryEconomicActivities.Repositories;
using OpenCnpj.Application.CompanyContacts.Repositories;
using OpenCnpj.Application.CompanySpecialSituations.Repositories;
using OpenCnpj.Application.Contacts.Repositories;
using OpenCnpj.Application.Countries.Repositories;
using OpenCnpj.Application.EconomicActivities.Repositories;
using OpenCnpj.Application.LegalNatures.Repositories;
using OpenCnpj.Application.LegalRepresentatives.Repositories;
using OpenCnpj.Application.Meis.Repositories;
using OpenCnpj.Application.Partners.Repositories;
using OpenCnpj.Application.PartnersQualifications.Repositories;
using OpenCnpj.Application.Phones.Repositories;
using OpenCnpj.Application.Reasons.Repositories;
using OpenCnpj.Application.Simples.Repositories;
using OpenCnpj.Application.SpecialSituations.Repositories;

namespace OpenCnpj.ConsoleApp.DependencyInjection;
public static class RepositoriesInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IBatchRepository, BatchRepository>();
        services.AddTransient<ICityRepository, CityRepository>();
        services.AddTransient<IEconomicActivityRepository, EconomicActivityRepository>();
        services.AddTransient<ICountryRepository, CountryRepository>();
        services.AddTransient<ILegalNatureRepository, LegalNatureRepository>();
        services.AddTransient<IPartnerQualificationRepository, PartnerQualificationRepository>();
        services.AddTransient<IReasonRepository, ReasonRepository>();
        services.AddTransient<ICompanyRepository, CompanyRepository>();
        services.AddTransient<IBatchFileRepository, BatchFileRepository>();
        services.AddTransient<IAddressTypeRepository, AddressTypeRepository>();
        services.AddTransient<IAddressRepository, AddressRepository>();
        services.AddTransient<ISpecialSituationRepository, SpecialSituationRepository>();
        services.AddTransient<IEconomicActivityRepository, EconomicActivityRepository>();
        services.AddTransient<ICompanySpecialSituationRepository, CompanySpecialSituationRepository>();
        services.AddTransient<ILegalRepresentativeRepository, LegalRepresentativeRepository>();
        services.AddTransient<IPartnerRepository, PartnerRepository>();
        services.AddTransient<IMeiRepository, MeiRepository>();
        services.AddTransient<ISimpleRepository, SimpleRepository>();
        services.AddTransient<IPhoneRepository, PhoneRepository>();
        services.AddTransient<IContactRepository, ContactRepository>();
        services.AddTransient<ICompanyContactRepository, CompanyContactRepository>();
        services.AddTransient<ICompanySecondaryEconomicActivityRepository, CompanySecondaryEconomicActivityRepository>();

        return services;
    }
}
