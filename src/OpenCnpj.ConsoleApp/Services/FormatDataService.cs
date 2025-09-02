using CSharpFunctionalExtensions;
using OpenCnpj.ConsoleApp.Application.Cities.Services;
using OpenCnpj.ConsoleApp.Application.Companies.Services;
using OpenCnpj.ConsoleApp.Application.Countries.Services;
using OpenCnpj.ConsoleApp.Application.EconomicActivities.Services;
using OpenCnpj.ConsoleApp.Application.LegalNatures.Services;
using OpenCnpj.ConsoleApp.Application.PartnersQualifications.Services;
using OpenCnpj.ConsoleApp.Application.Reasons.Services;
using OpenCnpj.ConsoleApp.Services.Interfaces;
using Serilog;

namespace OpenCnpj.ConsoleApp.Services;
public class FormatDataService(ICityService cityService, IEconomicActivityService economicActivityService, ICountryServices countryServices, ILegalNatureService legalNature, IPartnerQualificationService partnerQualificationService, IReasonService reasonService, ICompanyService companyService, ILogger logger) : IFormatDataService
{
    private readonly ILogger _logger = logger.ForContext<FormatDataService>();
    public async Task<Result> FormatData(CancellationToken cancellationToken)
    {
        try
        {
            var createCitiesResult = await cityService.CreateCities(cancellationToken);
            if (createCitiesResult.IsFailure)
                return createCitiesResult;

            var createCountriesResult = await countryServices.CreateCountries(cancellationToken);
            if (createCountriesResult.IsFailure)
                return createCountriesResult;

            var createEconomicActivitiesResult = await economicActivityService.CreateEconomicActivities(cancellationToken);
            if (createEconomicActivitiesResult.IsFailure)
                return createEconomicActivitiesResult;

            var createLegalNaturesResult = await legalNature.CreateLegalNatures(cancellationToken);
            if (createLegalNaturesResult.IsFailure)
                return createLegalNaturesResult;

            var createPartnerQualificationsResult = await partnerQualificationService.CreatePartnerQualifications(cancellationToken);
            if (createPartnerQualificationsResult.IsFailure)
                return createPartnerQualificationsResult;

            var createReasonsResult = await reasonService.CreateReasons(cancellationToken);
            if (createReasonsResult.IsFailure)
                return createReasonsResult;

            var createCompaniesResult = await companyService.CreateCompanies(cancellationToken);
            if (createCompaniesResult.IsFailure)
                return createCompaniesResult;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An unhandled exception occured while trying to format the data.");

            return Result.Failure("An unhandled error occured while trying to format the data.");
        }

        return Result.Success();
    }
} 
