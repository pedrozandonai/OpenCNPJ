using CSharpFunctionalExtensions;
using MongoDB.Driver;
using OpenCnpj.ConsoleApp.Application.PartnersQualifications.Domain;
using OpenCnpj.ConsoleApp.Application.PartnersQualifications.Repositories;
using OpenCnpj.ConsoleApp.Application.RawRecords;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;
using Serilog;
using System.Runtime.CompilerServices;

namespace OpenCnpj.ConsoleApp.Application.PartnersQualifications.Services;
public class PartnerQualificationService(IMongoDatabaseFactory mongoDatabaseFactory, IPartnerQualificationRepository partnerQualificationRepository, ILogger logger) : IPartnerQualificationService
{
    private readonly ILogger _logger = logger.ForContext<PartnerQualificationService>();
    public async Task<Result> CreatePartnerQualifications(CancellationToken cancellationToken)
    {
        try
        {
            await partnerQualificationRepository.DatabaseFactory.BeginAsync();

            await foreach (var partnerQualificationRawRecordRecord in GetAllPartnerQualificationRawRecords(cancellationToken))
            {
                if (!long.TryParse(partnerQualificationRawRecordRecord.Code, out var code))
                {
                    _logger
                        .ForContext("Code", partnerQualificationRawRecordRecord.Code, false)
                        .Warning("Unable to cast partner qualification code string to long.");
                    continue;
                }
                await partnerQualificationRepository.Insert(PartnerQualification.Create(code, partnerQualificationRawRecordRecord.Description), cancellationToken);
            }

            await partnerQualificationRepository.DatabaseFactory.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An exception occured while trying to create 'PartnerQualification' domain.");

            return Result.Failure("An error occured while trying to create 'PartnerQualification' domain.");
        }

        return Result.Success();
    }

    private async IAsyncEnumerable<PartnerQualificationRawRecord> GetAllPartnerQualificationRawRecords([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var citiesRawRecords = mongoDatabaseFactory.Database.GetCollection<PartnerQualificationRawRecord>("PartnerQualificationRawRecord");

        using var cursor = await citiesRawRecords.FindAsync(FilterDefinition<PartnerQualificationRawRecord>.Empty, cancellationToken: cancellationToken);
        while (await cursor.MoveNextAsync(cancellationToken))
        {
            foreach (var record in cursor.Current)
                yield return record;
        }
    }
}