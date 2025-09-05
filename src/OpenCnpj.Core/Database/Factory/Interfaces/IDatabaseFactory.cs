using System.Data;

namespace OpenCnpj.Core.Database.Factory.Interfaces;
public interface IDatabaseFactory : IDisposable
{
    public string ConnectionString { get; }
    public IDbConnection Connection { get; }
    public IDbTransaction? Transaction { get; }
    bool TransactionIsOpen { get; }
    void Begin();
    Task BeginAsync();
    void Commit();
    Task CommitAsync();
    void Rollback();
    Task RollbackAsync();
}
