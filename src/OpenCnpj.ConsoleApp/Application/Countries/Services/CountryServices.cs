using CSharpFunctionalExtensions;
using MongoDB.Driver;
using OpenCnpj.ConsoleApp.Application.Cities.Domain;
using OpenCnpj.ConsoleApp.Application.Countries.Domain;
using OpenCnpj.ConsoleApp.Application.Countries.Repositories;
using OpenCnpj.ConsoleApp.Application.RawRecords;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;
using Serilog;
using System.Runtime.CompilerServices;

namespace OpenCnpj.ConsoleApp.Application.Countries.Services;
public class CountryServices(IMongoDatabaseFactory mongoDatabaseFactory, ICountryRepository countryRepository, ILogger logger) : ICountryServices
{
    private readonly ILogger _logger = logger.ForContext<CountryServices>();
    public async Task<Result> CreateCountries(CancellationToken cancellationToken)
    {
        try
        {
            await countryRepository.DatabaseFactory.BeginAsync();

            await foreach (var countryRawRecord in GetAllCountriesRawRecords(cancellationToken))
                await countryRepository.Insert(Country.Create(countryRawRecord.Description), cancellationToken);

            await countryRepository.DatabaseFactory.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An exception occured while trying to create 'Country' domain.");

            return Result.Failure("An error occured while trying to create 'Country' domain.");
        }

        return Result.Success();
    }

    private async IAsyncEnumerable<CountryRawRecord> GetAllCountriesRawRecords([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var citiesRawRecords = mongoDatabaseFactory.Database.GetCollection<CountryRawRecord>("CountryRawRecord");

        using var cursor = await citiesRawRecords.FindAsync(FilterDefinition<CountryRawRecord>.Empty, cancellationToken: cancellationToken);
        while (await cursor.MoveNextAsync(cancellationToken))
        {
            foreach (var record in cursor.Current)
                yield return record;
        }
    }
}
