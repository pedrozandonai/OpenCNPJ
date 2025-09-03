using CSharpFunctionalExtensions;
using MongoDB.Driver;
using OpenCnpj.ConsoleApp.Application.Cities.Domain;
using OpenCnpj.ConsoleApp.Application.Cities.Repositories;
using OpenCnpj.ConsoleApp.Application.RawRecords;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;
using Serilog;
using System.Runtime.CompilerServices;

namespace OpenCnpj.ConsoleApp.Application.Cities.Services;
public class CityService(IMongoDatabaseFactory mongoDatabaseFactory, ICityRepository cityRepository, ILogger logger) : ICityService
{
    private readonly ILogger _logger = logger.ForContext<CityService>();
    public async Task<Result> CreateCities(CancellationToken cancellationToken)
    {
        try
        {
            await cityRepository.DatabaseFactory.BeginAsync();

            await foreach (var cityRawRecord in GetAllCitiesRawRecords(cancellationToken))
            {
                if (!long.TryParse(cityRawRecord.Code, out var code))
                {
                    _logger
                        .ForContext("Code", cityRawRecord.Code, false)
                        .Warning("Unable to cast city code string to long.");
                    continue;
                }

                await cityRepository.Insert(City.Create(code, cityRawRecord.Description), cancellationToken);
            }

            await cityRepository.DatabaseFactory.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An exception occured while trying to create 'Cities' domain.");

            return Result.Failure("An error occured while trying to create 'cities' domain.");
        }

        return Result.Success();
    }

    private async IAsyncEnumerable<CityRawRecord> GetAllCitiesRawRecords([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var citiesRawRecords = mongoDatabaseFactory.Database.GetCollection<CityRawRecord>("CityRawRecord");

        using var cursor = await citiesRawRecords.FindAsync(FilterDefinition<CityRawRecord>.Empty, cancellationToken: cancellationToken);

        while (await cursor.MoveNextAsync(cancellationToken))
        {
            foreach (var record in cursor.Current)
                yield return record;
        }
    }
}
