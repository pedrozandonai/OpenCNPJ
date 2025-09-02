using Dapper;
using OpenCnpj.ConsoleApp.Application.Reasons.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.Reasons.Repositories;
public class ReasonRepository(IDatabaseFactory databaseFactory) : IReasonRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;
    public async Task Insert(Reason reason, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO reasons (code,
                                                  description)
                                          VALUES (@Code,
                                                  @Description)";

        var command = new CommandDefinition(sql, reason, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        await DatabaseFactory.Connection.ExecuteAsync(command);
    }
}
