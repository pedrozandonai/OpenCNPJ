using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using OpenCnpj.Application.Companies.Models;
using OpenCnpj.Application.Companies.Repositories;
using OpenCnpj.Application.Meis.Domain;
using OpenCnpj.Application.Meis.Repositories;
using OpenCnpj.Application.RawRecords;
using OpenCnpj.Application.Simples.Domain;
using OpenCnpj.Application.Simples.Repositories;
using OpenCnpj.Core.Configurations;
using OpenCnpj.Core.Database.Factory.Interfaces;
using OpenCnpj.Core.Helpers;
using Serilog;
using System.Collections.Concurrent;

namespace OpenCnpj.Application.Simples.Services;
public class SimpleService(IMongoDatabaseFactory mongoDatabaseFactory, IServiceProvider serviceProvider, TweakSettings tweakSettings, ILogger logger) : ISimpleService
{
    private readonly IdGenerator _simpleIdGen = new();
    private readonly IdGenerator _meiIdGen = new();
    private readonly ConcurrentBag<Simple> _simplesToInsert = [];
    private readonly ConcurrentBag<Mei> _meisToInsert = [];

    private readonly ILogger _logger = logger.ForContext<SimpleService>();
    public async Task<Result> CreateSimples(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateAsyncScope();

        try
        {
            var simplesCollection = mongoDatabaseFactory.Database.GetCollection<SimpleDataRawRecord>("SimplesDataRaw");

            var meiRepository = scope.ServiceProvider.GetRequiredService<IMeiRepository>();
            var simpleRepository = scope.ServiceProvider.GetRequiredService<ISimpleRepository>();

            int pageSize = tweakSettings.FormatRawDataSettings.RecordsBatchAmount;
            var page = 0;

            while (true)
            {
                _logger.Information("Current page: {0}", page);

                var pipeline = simplesCollection.Aggregate()
                    .Skip(page * pageSize)
                    .Limit(pageSize);

                using var cursor = await pipeline.ToCursorAsync(cancellationToken);
                var anyInPage = false;
                var tasks = new List<Task>();

                while (await cursor.MoveNextAsync(cancellationToken))
                {
                    foreach (var simpleDataRawRecord in cursor.Current)
                    {
                        anyInPage = true;

                        tasks.Add(ProcessSimples(simpleDataRawRecord, cancellationToken));
                    }
                }

                await Task.WhenAll(tasks);

                var dbFactory = scope.ServiceProvider.GetRequiredService<IDatabaseFactory>();

                await dbFactory.BeginAsync();

                if (!_meisToInsert.IsEmpty)
                {
                    await meiRepository.CopyToTable(_meisToInsert, cancellationToken);

                    _meisToInsert.Clear();
                }

                if (!_simplesToInsert.IsEmpty)
                {
                    await simpleRepository.CopyToTable(_simplesToInsert, cancellationToken);

                    _simplesToInsert.Clear();
                }

                await dbFactory.CommitAsync(); 
                
                if (!anyInPage)
                    break;

                page++;
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An exception occurred while trying to create the SIMPLES data.");
            return Result.Failure("An exception occurred while trying to create the SIMPLES data.");
        }

        return Result.Success();
    }

    private async Task ProcessSimples(SimpleDataRawRecord simpleDataRawRecord, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        var companyRepository = serviceProvider.GetRequiredService<ICompanyRepository>();

        var company = await companyRepository.GetByBasicCnpj(simpleDataRawRecord.BasicCnpj, cancellationToken);
        if (company == null)
        {
            _logger.Warning("Unable to retreive company by basic cnpj.");
            return;
        }

        Mei? mei = null;
        if (simpleDataRawRecord.OptInMei.HasValue && simpleDataRawRecord.OptInMei.Value)
            mei = Mei.Create(_meiIdGen.NextId(), simpleDataRawRecord.MeiOptionDate, simpleDataRawRecord.MeiExclusionDate);

        var simple = Simple.Create(_simpleIdGen.NextId(), company.ID, mei?.ID, simpleDataRawRecord.OptInSimple, simpleDataRawRecord.SimpleOptionDate, simpleDataRawRecord.SimpleExclusionDate);

        if (mei != null)
            _meisToInsert.Add(mei);

        _simplesToInsert.Add(simple);
    }
}
