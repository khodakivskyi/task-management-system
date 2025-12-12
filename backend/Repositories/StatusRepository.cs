using backend.Interfaces;
using backend.Models;
using Dapper;

namespace backend.Repositories;

/// <summary>
/// Repository for Status entity operations
/// </summary>
public class StatusRepository : BaseRepository, IRepository<Status>
{
    public StatusRepository(string connectionString) : base(connectionString) { }

    public async Task<Status?> GetByIdAsync(int id)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<Status>(
            @"SELECT ""Id"", ""Name"", ""Color""
              FROM ""Statuses""
              WHERE ""Id"" = @Id",
            new { Id = id });
    }

    public async Task<IEnumerable<Status>> GetAllAsync()
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryAsync<Status>(
            @"SELECT ""Id"", ""Name"", ""Color""
              FROM ""Statuses""
              ORDER BY ""Name""");
    }

    public async Task<int> CreateAsync(Status entity)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QuerySingleAsync<int>(
            @"INSERT INTO ""Statuses"" (""Name"", ""Color"")
              VALUES (@Name, @Color)
              RETURNING ""Id""",
            new { entity.Name, entity.Color });
    }

    public async Task<bool> UpdateAsync(Status entity)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"UPDATE ""Statuses""
              SET ""Name"" = @Name, ""Color"" = @Color
              WHERE ""Id"" = @Id",
            new { entity.Id, entity.Name, entity.Color });
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"DELETE FROM ""Statuses""
              WHERE ""Id"" = @Id",
            new { Id = id });
        return affected > 0;
    }
}





