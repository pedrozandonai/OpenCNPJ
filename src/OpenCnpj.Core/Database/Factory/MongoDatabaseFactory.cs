using MongoDB.Driver;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Core.Database.Factory;

public class MongoDatabaseFactory : IMongoDatabaseFactory
{
    private readonly MongoClient _client;
    private readonly IMongoDatabase _database;
    private IClientSessionHandle? _session;

    public MongoClient Client => _client;
    public bool TransactionIsOpen { get; private set; }

    public MongoDatabaseFactory(string connectionString, string databaseName)
    {
        var settings = MongoClientSettings.FromConnectionString(connectionString);
        settings.SocketTimeout = TimeSpan.FromMinutes(10);
        settings.ConnectTimeout = TimeSpan.FromMinutes(10);
        _client = new MongoClient(settings);
        _database = _client.GetDatabase(databaseName);
        TransactionIsOpen = false;
    }

    public IMongoDatabase Database => _database;
    public IClientSessionHandle? Session => _session;

    public void Begin()
    {
        _session = _client.StartSession();
        _session.StartTransaction();
        TransactionIsOpen = true;
    }

    public async Task BeginAsync()
    {
        _session = await _client.StartSessionAsync();
        _session.StartTransaction();
        TransactionIsOpen = true;
    }

    public void Commit()
    {
        _session?.CommitTransaction();
        TransactionIsOpen = false;
    }

    public async Task CommitAsync()
    {
        if (_session == null)
            throw new Exception("No transaction opened.");

        await _session.CommitTransactionAsync();
        TransactionIsOpen = false;
    }

    public void Rollback()
    {
        _session?.AbortTransaction();
        TransactionIsOpen = false;
    }

    public async Task RollbackAsync()
    {
        if (_session != null)
            await _session.AbortTransactionAsync();

        TransactionIsOpen = false;
    }

    public void Dispose()
    {
        if (TransactionIsOpen)
            _session?.AbortTransaction();

        _session?.Dispose();
        GC.SuppressFinalize(this);
    }
}