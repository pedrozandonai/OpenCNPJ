using System.Data;

namespace OpenCnpj.Core.Database.Factory.Interfaces;
public interface IDatabaseFactory
{
    string ConnectionString { get; }
    IDbConnection CreateConnection();
    Task<IDbConnection> CreateConnectionAsync();
    void Dispose();
}