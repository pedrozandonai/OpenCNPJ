using CSharpFunctionalExtensions;
using MongoDB.Driver;
using OpenCnpj.Application.Cities.Domain;
using OpenCnpj.Application.Countries.Domain;
using OpenCnpj.Application.RawRecords;
using OpenCnpj.Core.Database.Factory.Interfaces;
using Serilog;
using System.Runtime.CompilerServices;

namespace OpenCnpj.Application.Countries.Services;
public class CountryServices(IMongoDatabaseFactory mongoDatabaseFactory, ICountryRepository countryRepository, ILogger logger) : ICountryServices
{
    private readonly ILogger _logger = logger.ForContext<CountryServices>();
    public async Task<Result> CreateCountries(CancellationToken cancellationToken)
    {
        try
        {
            await countryRepository.Database.BeginTransactionAsync(cancellationToken);

            List<Country> countries = [];
            await foreach (var countryRawRecord in GetAllCountriesRawRecords(cancellationToken))
                countries.Add(Country.Create(countryRawRecord.Code, countryRawRecord.Description));

            await countryRepository.Insert(countries, cancellationToken);

            await countryRepository.SaveAllChanges(cancellationToken);

            await countryRepository.Database.CommitTransactionAsync(cancellationToken);
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
