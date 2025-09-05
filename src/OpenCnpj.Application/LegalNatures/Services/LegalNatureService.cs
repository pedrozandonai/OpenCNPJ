using CSharpFunctionalExtensions;
using MongoDB.Driver;
using OpenCnpj.Application.LegalNatures.Domain;
using OpenCnpj.Application.LegalNatures.Repositories;
using OpenCnpj.Application.RawRecords;
using OpenCnpj.Core.Database.Factory.Interfaces;
using Serilog;
using System.Runtime.CompilerServices;

namespace OpenCnpj.Application.LegalNatures.Services;
public class LegalNatureService(IMongoDatabaseFactory mongoDatabaseFactory, ILegalNatureRepository legalNatureRepository, ILogger logger) : ILegalNatureService
{
    private readonly ILogger _logger = logger.ForContext<LegalNatureService>();
    public async Task<Result> CreateLegalNatures(CancellationToken cancellationToken)
    {
        try
        {
            await legalNatureRepository.DatabaseFactory.BeginAsync();

            await foreach (var legalNatureRawRecordRecord in GetAllLegalNatureRawRecords(cancellationToken))
                await legalNatureRepository.Insert(LegalNature.Create(legalNatureRawRecordRecord.Code, legalNatureRawRecordRecord.Description), cancellationToken);

            await legalNatureRepository.DatabaseFactory.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An exception occured while trying to create 'LegalNature' domain.");

            return Result.Failure("An error occured while trying to create 'LegalNature' domain.");
        }

        return Result.Success();
    }

    private async IAsyncEnumerable<LegalNatureRawRecord> GetAllLegalNatureRawRecords([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var citiesRawRecords = mongoDatabaseFactory.Database.GetCollection<LegalNatureRawRecord>("LegalNatureRawRecord");

        using var cursor = await citiesRawRecords.FindAsync(FilterDefinition<LegalNatureRawRecord>.Empty, cancellationToken: cancellationToken);
        while (await cursor.MoveNextAsync(cancellationToken))
        {
            foreach (var record in cursor.Current)
                yield return record;
        }
    }
}
