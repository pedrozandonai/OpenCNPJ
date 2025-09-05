using MongoDB.Driver;

namespace OpenCnpj.Core.Database.Factory.Interfaces;
public interface IMongoDatabaseFactory : IDisposable
{
    MongoClient Client { get; }
    IMongoDatabase Database { get; }
    IClientSessionHandle? Session { get; }
    bool TransactionIsOpen { get; }
    void Begin();
    Task BeginAsync();
    void Commit();
    Task CommitAsync();
    void Dispose();
    void Rollback();
    Task RollbackAsync();
}