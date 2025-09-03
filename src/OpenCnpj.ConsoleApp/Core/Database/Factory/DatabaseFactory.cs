using Npgsql;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;
using System.Data;

public class DatabaseFactory : IDatabaseFactory, IDisposable
{
    private readonly string _connectionString;
    private NpgsqlConnection? _connection;
    private NpgsqlTransaction? _transaction;

    public DatabaseFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public bool TransactionIsOpen => _transaction != null;
    public string ConnectionString => _connectionString;
    public IDbConnection Connection => _connection ?? throw new InvalidOperationException("Connection not opened");
    public IDbTransaction? Transaction => _transaction;

    private async Task EnsureConnectionOpenAsync()
    {
        if (_connection == null)
        {
            _connection = new NpgsqlConnection(_connectionString);
            await _connection.OpenAsync();
        }
        else if (_connection.State != ConnectionState.Open)
        {
            await _connection.OpenAsync();
        }
    }

    public void Begin()
    {
        EnsureConnectionOpenAsync().GetAwaiter().GetResult();
        _transaction = _connection!.BeginTransaction();
    }

    public async Task BeginAsync()
    {
        await EnsureConnectionOpenAsync();
        _transaction = await _connection!.BeginTransactionAsync();
    }

    public void Commit()
    {
        if (_transaction == null) throw new Exception("No transaction opened.");
        _transaction.Commit();
        _transaction = null;
    }

    public async Task CommitAsync()
    {
        if (_transaction == null) throw new Exception("No transaction opened.");
        await _transaction.CommitAsync();
        _transaction = null;
    }

    public void Rollback()
    {
        if (_transaction == null) throw new Exception("No transaction opened.");
        _transaction.Rollback();
        _transaction = null;
    }

    public async Task RollbackAsync()
    {
        if (_transaction == null) throw new Exception("No transaction opened.");
        await _transaction.RollbackAsync();
        _transaction = null;
    }

    public void Dispose()
    {
        try { _transaction?.Rollback(); } catch { }
        _transaction?.Dispose();
        try { _connection?.Close(); } catch { }
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }
}
