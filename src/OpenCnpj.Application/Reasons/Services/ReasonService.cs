using CSharpFunctionalExtensions;
using MongoDB.Driver;
using OpenCnpj.Application.RawRecords;
using OpenCnpj.Application.Reasons.Domain;
using OpenCnpj.Application.Reasons.Repositories;
using OpenCnpj.Core.Database.Factory.Interfaces;
using Serilog;
using System.Runtime.CompilerServices;

namespace OpenCnpj.Application.Reasons.Services;
public class ReasonService(IMongoDatabaseFactory mongoDatabaseFactory, IReasonRepository reasonRepository, ILogger logger) : IReasonService
{
    private readonly ILogger _logger = logger.ForContext<ReasonService>();
    public async Task<Result> CreateReasons(CancellationToken cancellationToken)
    {
        try
        {
            await reasonRepository.DatabaseFactory.BeginAsync();

            await foreach (var reasonRawRecordRecord in GetAllReasonsRawRecords(cancellationToken))
                await reasonRepository.Insert(Reason.Create(reasonRawRecordRecord.Code, reasonRawRecordRecord.Description), cancellationToken);

            await reasonRepository.DatabaseFactory.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An exception occured while trying to create 'Reason' domain.");

            return Result.Failure("An error occured while trying to create 'Reason' domain.");
        }

        return Result.Success();
    }

    private async IAsyncEnumerable<ReasonRawRecord> GetAllReasonsRawRecords([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var citiesRawRecords = mongoDatabaseFactory.Database.GetCollection<ReasonRawRecord>("ReasonRawRecord");

        using var cursor = await citiesRawRecords.FindAsync(FilterDefinition<ReasonRawRecord>.Empty, cancellationToken: cancellationToken);
        while (await cursor.MoveNextAsync(cancellationToken))
        {
            foreach (var record in cursor.Current)
                yield return record;
        }
    }
}
