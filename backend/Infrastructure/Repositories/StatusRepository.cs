using backend.Exceptions;
using backend.Interfaces;
using backend.Models;
using Dapper;

namespace backend.Infrastructure.Repositories;

/// <summary>
/// Repository for Status entity operations (read-only)
/// Statuses are default values in the database and cannot be created, updated, or deleted
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
        throw new ForbiddenException("Statuses are default values and cannot be created. They must be defined in the database.");
    }

    public async Task<bool> UpdateAsync(Status entity)
    {
        throw new ForbiddenException("Statuses are default values and cannot be updated. They must be modified in the database.");
    }

    public async Task<bool> DeleteAsync(int id)
    {
        throw new ForbiddenException("Statuses are default values and cannot be deleted. They must be removed from the database.");
    }
}
