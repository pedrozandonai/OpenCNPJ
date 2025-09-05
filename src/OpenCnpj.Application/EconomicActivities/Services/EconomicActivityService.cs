using CSharpFunctionalExtensions;
using MongoDB.Driver;
using OpenCnpj.Application.EconomicActivities.Domain;
using OpenCnpj.Application.EconomicActivities.Repositories;
using OpenCnpj.Application.RawRecords;
using OpenCnpj.Core.Database.Factory.Interfaces;
using Serilog;
using System.Runtime.CompilerServices;

namespace OpenCnpj.Application.EconomicActivities.Services;
public class EconomicActivityService(IEconomicActivityRepository economicActivityRepository, IMongoDatabaseFactory mongoDatabaseFactory, ILogger logger) : IEconomicActivityService
{
    private readonly ILogger _logger = logger.ForContext<EconomicActivityService>();

    public async Task<Result> CreateEconomicActivities(CancellationToken cancellationToken)
    {
        try
        {
            await economicActivityRepository.DatabaseFactory.BeginAsync();

            await foreach (var economicActivityRawRecord in GetAllEconomicActivitiesRawRecords(cancellationToken))
                await economicActivityRepository.Insert(EconomicActivity.Create(economicActivityRawRecord.Code, economicActivityRawRecord.Description), cancellationToken);

            await economicActivityRepository.DatabaseFactory.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An exception occured while trying to create 'EconomicActivity' domain.");

            return Result.Failure("An error occured while trying to create 'EconomicActivity' domain.");
        }

        return Result.Success();
    }

    private async IAsyncEnumerable<CnaeRawRecord> GetAllEconomicActivitiesRawRecords([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var economicActivitiesRawRecords = mongoDatabaseFactory.Database.GetCollection<CnaeRawRecord>("CnaeRawRecord");

        using var cursor = await economicActivitiesRawRecords.FindAsync(FilterDefinition<CnaeRawRecord>.Empty, cancellationToken: cancellationToken);
        while (await cursor.MoveNextAsync(cancellationToken))
        {
            foreach (var record in cursor.Current)
                yield return record;
        }
    }
}
