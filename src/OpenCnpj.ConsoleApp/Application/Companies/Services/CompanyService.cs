using CSharpFunctionalExtensions;
using MongoDB.Driver;
using OpenCnpj.ConsoleApp.Application.Addresses.Services;
using OpenCnpj.ConsoleApp.Application.Companies.Domain;
using OpenCnpj.ConsoleApp.Application.Companies.Models;
using OpenCnpj.ConsoleApp.Application.Companies.Repositories;
using OpenCnpj.ConsoleApp.Application.Countries.Repositories;
using OpenCnpj.ConsoleApp.Application.EconomicActivities.Repositories;
using OpenCnpj.ConsoleApp.Application.LegalNatures.Repositories;
using OpenCnpj.ConsoleApp.Application.PartnersQualifications.Repositories;
using OpenCnpj.ConsoleApp.Application.RawRecords;
using OpenCnpj.ConsoleApp.Application.SpecialSituations.Domain;
using OpenCnpj.ConsoleApp.Application.SpecialSituations.Repositories;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;
using Serilog;

namespace OpenCnpj.ConsoleApp.Application.Companies.Services;
public class CompanyService(IMongoDatabaseFactory mongoDatabaseFactory, ICompanyRepository companyRepository, IAddressService addressService, ILegalNatureRepository legalNatureRepository, IPartnerQualificationRepository partnerQualificationRepository, ICountryRepository countryRepository, ISpecialSituationRepository specialSituationRepository, IEconomicActivityRepository economicActivityRepository, ILogger logger) : ICompanyService
{
    private readonly ILogger _logger = logger.ForContext<CompanyService>();
    public async Task<Result> CreateCompanies(CancellationToken cancellationToken)
    {
        try
        {
            await companyRepository.DatabaseFactory.BeginAsync();

            var companiesCollection = mongoDatabaseFactory.Database.GetCollection<CompanyRawRecord>("CompaniesRaw");

            const int pageSize = 1000;
            var page = 0;

            while (true)
            {
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
                var any = false;

                while (await cursor.MoveNextAsync(cancellationToken))
                {
                    foreach (var companyRawRecord in cursor.Current)
                    {
                        any = true;

                        foreach (var establishmentRawRecord in companyRawRecord.Establishments)
                        {
                            var address = await addressService.CreateAddressByEstablishmentRawRecord(establishmentRawRecord, cancellationToken);
                            if (address.IsFailure)
                                _logger.Warning(address.Error);

                            var legalNature = await legalNatureRepository.GetByCode(companyRawRecord.LegalNatureCode, cancellationToken);
                            if (legalNature == null)
                            {
                                _logger.Warning("Unable to fetch the legal nature of the company.");
                                continue;
                            }

                            var mainPartnerQualificationID = await partnerQualificationRepository.GetByCode(companyRawRecord.LegalNatureCode, cancellationToken);
                            if (mainPartnerQualificationID == null)
                            {
                                _logger.Warning("Unable to fetch the main partner qualification of the company.");
                                continue;
                            }
                            
                            var country = await countryRepository.GetByCode(establishmentRawRecord.CountryCode, cancellationToken);
                            if (country == null)
                            {
                                _logger.Warning("Unable to fetch the country of the company.");
                                continue;
                            }

                            long? specialSituationID = null;
                            if (!string.IsNullOrEmpty(establishmentRawRecord.SpecialStatus))
                            {
                                specialSituationID = await specialSituationRepository.Insert(SpecialSituation.Create(establishmentRawRecord.SpecialStatus, establishmentRawRecord.SpecialStatusDate!.Value), cancellationToken);
                            }
                            
                            var mainEconomicActivity = await economicActivityRepository.GetByCode(establishmentRawRecord.MainCnae, cancellationToken);
                            if (mainEconomicActivity == null)
                            {
                                _logger.Warning("Unable to fetch the country of the company.");
                                continue;
                            }

                            string identifier = string.Format("{0}{1}{2}", establishmentRawRecord.BasicCnpj, establishmentRawRecord.OrderCnpj, establishmentRawRecord.CheckDigitCnpj);

                            var company = Company.Create(legalNature.ID, mainPartnerQualificationID.ID, (int)companyRawRecord.CompanySize, (int)establishmentRawRecord.HeadOfficeOrBranch, country.ID, address.Value.ID, specialSituationID, mainEconomicActivity.ID, identifier, companyRawRecord.CorporateName, companyRawRecord.ShareCapital, companyRawRecord.ResponsibleFederativeEntity, establishmentRawRecord.TradeName, DateOnly.FromDateTime(establishmentRawRecord.RegistrationStatusDate.Value), establishmentRawRecord.ForeignCityName, establishmentRawRecord.StartActivityDate.Value);
                            
                            await companyRepository.Insert(company, cancellationToken);
                        }
                    }
                }

                if (!any) break; // acabou
                page++;
            }

            await companyRepository.DatabaseFactory.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An exception occured while trying to create 'Company' domain.");

            return Result.Failure("An error occured while trying to create 'Company' domain.");
        }

        return Result.Success();
    }
}
