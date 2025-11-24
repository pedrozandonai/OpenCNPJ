using Microsoft.Data.Sqlite;
using OpenCnpj.Core.Database.Factory.Interfaces;
using System.Data;
using System.Data.Common;

namespace OpenCnpj.Core.Database.Factory;
public class DatabaseFactory : IDatabaseFactory, IDisposable
{
    private readonly string _connectionString;
    private bool _disposed;

    public DatabaseFactory(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentNullException(nameof(connectionString));

        _connectionString = connectionString;
    }

    public string ConnectionString => _connectionString;

    /// <summary>
    /// Creates and opens a NEW SQLite connection (stateless).
    /// </summary>
    public IDbConnection CreateConnection()
    {
        var conn = new SqliteConnection(_connectionString);
        conn.Open();
        ApplyPragma(conn);
        return conn;
    }

    /// <summary>
    /// Creates and opens a NEW SQLite connection asynchronously (stateless).
    /// </summary>
    public async Task<IDbConnection> CreateConnectionAsync()
    {
        var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        await ApplyPragmaAsync(conn);
        return conn;
    }

    private void ApplyPragma(DbConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "pragma busy_timeout = 5000;";
        command.ExecuteNonQuery();
    }

    private async Task ApplyPragmaAsync(DbConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "pragma busy_timeout = 5000;";
        await command.ExecuteNonQueryAsync();
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
    }
}

