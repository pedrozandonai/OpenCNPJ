using Dapper;
using OpenCnpj.ConsoleApp.Application.SpecialSituations.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.SpecialSituations.Repositories;

public class SpecialSituationRepository(IDatabaseFactory databaseFactory) : ISpecialSituationRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;

    public async Task<long> Insert(SpecialSituation specialSituation, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO special_situations (description,
                                                             situation_date)
                                                     VALUES (@Description,
                                                             @SituationDate)
                                                  RETURNING ID";

        var command = new CommandDefinition(sql, specialSituation, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        return await DatabaseFactory.Connection.ExecuteScalarAsync<long>(command);
    }
}