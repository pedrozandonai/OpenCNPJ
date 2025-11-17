using System.Data;

namespace OpenCnpj.Core.Database.Factory.Interfaces;
public interface IDatabaseFactory
{
    IDbConnection Connection { get; }
    string ConnectionString { get; }
    IDbTransaction? Transaction { get; }

    void Begin(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    Task BeginAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    void Commit();
    Task CommitAsync();
    void Dispose();
    void Rollback();
    Task RollbackAsync();
}