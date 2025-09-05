using CSharpFunctionalExtensions;
using Npgsql;
using OpenCnpj.Core.Database.Factory.Interfaces;
using System.Data;

public class DatabaseFactory : IDatabaseFactory
{
    private string _connectionString { get; set; }
    private NpgsqlConnection _connection { get; set; }
    private NpgsqlTransaction? _transaction { get; set; }
    public bool TransactionIsOpen { get; set; }

    public DatabaseFactory(string connectionString)
    {
        _connection = new NpgsqlConnection(connectionString);
        _connectionString = connectionString;
        TransactionIsOpen = false;
        _connection.Open();
    }

    public string ConnectionString => _connectionString;
    public IDbConnection Connection => _connection;
    public IDbTransaction? Transaction => _transaction;

    public void Begin()
    {
        _transaction = _connection.BeginTransaction();
        TransactionIsOpen = true;
    }

    public async Task BeginAsync()
    {
        _transaction = await _connection.BeginTransactionAsync();
        TransactionIsOpen = true;
    }

    public void Commit()
    {
        _transaction!.Commit();
        TransactionIsOpen = false;
    }

    public async Task CommitAsync()
    {
        if (_transaction == null)
            throw new Exception("No transaction opened.");

        await _transaction!.CommitAsync();
        TransactionIsOpen = false;
    }

    public void Rollback()
    {
        _transaction!.Rollback();
        TransactionIsOpen = false;
    }

    public async Task RollbackAsync()
    {
        await _transaction!.RollbackAsync();
        TransactionIsOpen = false;
    }

    public void Dispose()
    {
        if (TransactionIsOpen)
            _transaction?.Rollback();

        _transaction?.Dispose();
        _connection?.Close();
        _connection?.Dispose();

        GC.SuppressFinalize(this);
    }

    public Result VerifyDatabaseTransaction()
    {
        if (TransactionIsOpen)
            return Result.Success();

        return Result.Failure("Not in transaction.");
    }
}