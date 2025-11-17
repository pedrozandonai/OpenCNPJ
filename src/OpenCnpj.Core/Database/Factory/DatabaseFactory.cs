using Microsoft.Data.Sqlite;
using OpenCnpj.Core.Database.Factory.Interfaces;
using System.Data;
using System.Data.Common;

namespace OpenCnpj.Core.Database.Factory;
public class DatabaseFactory : IDatabaseFactory
{
    private readonly string _connectionString;
    private DbConnection? _connection;
    private DbTransaction? _transaction;
    private bool _openTransaction;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private bool _disposed;

    public DatabaseFactory(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentNullException(nameof(connectionString));

        _connectionString = connectionString;
    }

    public string ConnectionString => _connectionString;

    public IDbConnection Connection
    {
        get
        {
            EnsureConnectionOpen();
            return _connection!;
        }
    }

    public IDbTransaction? Transaction => _transaction;

    protected DbConnection CreateConnection()
    {
        return new SqliteConnection(ConnectionString);
    }

    protected void OnConnectionOpened(DbConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "pragma busy_timeout = 5000;";
        command.ExecuteNonQuery();
    }

    protected async Task OnConnectionOpenedAsync(DbConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "pragma busy_timeout = 5000;";
        await command.ExecuteNonQueryAsync();
    }

    private void EnsureConnectionOpen()
    {
        if (_connection == null || _connection.State != ConnectionState.Open)
        {
            _semaphore.Wait();
            try
            {
                if (_connection == null || _connection.State != ConnectionState.Open)
                {
                    if (_connection != null && _connection.State == ConnectionState.Closed)
                    {
                        _connection.Dispose();
                        _connection = null;
                    }

                    _connection = CreateConnection();
                    _connection.Open();
                    OnConnectionOpened(_connection);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }

    private async Task EnsureConnectionOpenAsync()
    {
        if (_connection == null || _connection.State != ConnectionState.Open)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (_connection == null || _connection.State != ConnectionState.Open)
                {
                    if (_connection != null && _connection.State == ConnectionState.Closed)
                    {
                        _connection.Dispose();
                        _connection = null;
                    }

                    _connection = CreateConnection();
                    await _connection.OpenAsync();
                    await OnConnectionOpenedAsync(_connection);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }

    public void Begin(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        if (_openTransaction)
            throw new InvalidOperationException("A transaction is already active.");

        EnsureConnectionOpen();
        _transaction = _connection!.BeginTransaction(isolationLevel);
        _openTransaction = true;
    }

    public async Task BeginAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        if (_openTransaction)
            throw new InvalidOperationException("A transaction is already active.");

        await EnsureConnectionOpenAsync();
        _transaction = await _connection!.BeginTransactionAsync(isolationLevel);
        _openTransaction = true;
    }

    public void Commit()
    {
        if (_transaction == null)
            throw new InvalidOperationException("No active transaction to commit.");

        _transaction.Commit();
        _openTransaction = false;
    }

    public async Task CommitAsync()
    {
        if (_transaction == null)
            throw new InvalidOperationException("No active transaction to commit.");

        await _transaction.CommitAsync();
        _openTransaction = false;
    }

    public void Rollback()
    {
        if (_transaction == null)
            throw new InvalidOperationException("No active transaction to rollback.");

        _transaction.Rollback();
        _openTransaction = false;
    }

    public async Task RollbackAsync()
    {
        if (_transaction == null)
            throw new InvalidOperationException("No active transaction to rollback.");

        await _transaction.RollbackAsync();
        _openTransaction = false;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        if (_openTransaction && _transaction != null)
        {
            try
            {
                _transaction.Rollback();
            }
            catch
            {
                // Suppress exceptions during dispose
            }
        }

        _transaction?.Dispose();

        if (_connection != null)
        {
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
            _connection.Dispose();
        }

        _semaphore?.Dispose();

        _disposed = true;
    }
}

