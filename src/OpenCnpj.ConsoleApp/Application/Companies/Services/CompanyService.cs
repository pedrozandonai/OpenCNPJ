using CSharpFunctionalExtensions;
using MongoDB.Driver;
using OpenCnpj.ConsoleApp.Application.Companies.Repositories;
using OpenCnpj.ConsoleApp.Application.RawRecords;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;
using Serilog;
using System.Runtime.CompilerServices;

namespace OpenCnpj.ConsoleApp.Application.Companies.Services;
public class CompanyService(IMongoDatabaseFactory mongoDatabaseFactory, ICompanyRepository companyRepository, ILogger logger)
{
    private readonly ILogger _logger = logger.ForContext<CompanyService>();
    public async Task<Result> CreateCompanies(CancellationToken cancellationToken)
    {
        try
        {
            await companyRepository.DatabaseFactory.BeginAsync();

            await foreach (var companyRawRecord in GetAllCompanies(cancellationToken))
                throw new NotImplementedException();

            await companyRepository.DatabaseFactory.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An exception occured while trying to create 'Company' domain.");

            return Result.Failure("An error occured while trying to create 'Company' domain.");
        }

        return Result.Success();
    }

    private async IAsyncEnumerable<CompanyRawRecord> GetAllCompanies([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var citiesRawRecords = mongoDatabaseFactory.Database.GetCollection<CompanyRawRecord>("CompaniesRaw");

        using var cursor = await citiesRawRecords.FindAsync(FilterDefinition<CompanyRawRecord>.Empty, cancellationToken: cancellationToken);
        while (await cursor.MoveNextAsync(cancellationToken))
        {
            foreach (var record in cursor.Current)
                yield return record;
        }
    }
}
