using Dapper;
using Npgsql;

namespace backend.Repositories;

/// <summary>
/// Base repository class providing common database connection string
/// </summary>
public abstract class BaseRepository
{
    protected readonly string _connectionString;

    protected BaseRepository(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    protected async Task<NpgsqlConnection> GetConnectionAsync()
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}

