using CSharpFunctionalExtensions;
using MongoDB.Driver;
using OpenCnpj.ConsoleApp.Application.Addresses.Services;
using OpenCnpj.ConsoleApp.Application.Companies.Domain;
using OpenCnpj.ConsoleApp.Application.Companies.Models;
using OpenCnpj.ConsoleApp.Application.Companies.Repositories;
using OpenCnpj.ConsoleApp.Application.LegalNatures.Repositories;
using OpenCnpj.ConsoleApp.Application.RawRecords;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;
using Serilog;

namespace OpenCnpj.ConsoleApp.Application.Companies.Services;
public class CompanyService(IMongoDatabaseFactory mongoDatabaseFactory, ICompanyRepository companyRepository, IAddressService addressService, ILegalNatureRepository legalNatureRepository, ILogger logger) : ICompanyService
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



                            //var company = Company.Create(legalNature.ID, );
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
