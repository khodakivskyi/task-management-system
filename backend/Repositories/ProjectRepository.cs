using backend.Interfaces;
using backend.Models;
using Dapper;

namespace backend.Repositories;

/// <summary>
/// Repository for Project entity operations
/// </summary>
public class ProjectRepository : BaseRepository, IRepository<Project>
{
    public ProjectRepository(string connectionString) : base(connectionString) { }

    public async Task<Project?> GetByIdAsync(int id)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<Project>(
            @"SELECT ""Id"", ""OwnerId"", ""Name"", ""Description"", ""StartDate"", ""EndDate""
              FROM ""Projects""
              WHERE ""Id"" = @Id",
            new { Id = id });
    }

    public async Task<IEnumerable<Project>> GetAllAsync()
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryAsync<Project>(
            @"SELECT ""Id"", ""OwnerId"", ""Name"", ""Description"", ""StartDate"", ""EndDate""
              FROM ""Projects""
              ORDER BY ""StartDate"" DESC");
    }

    public async Task<int> CreateAsync(Project entity)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QuerySingleAsync<int>(
            @"INSERT INTO ""Projects"" (""OwnerId"", ""Name"", ""Description"", ""StartDate"", ""EndDate"")
              VALUES (@OwnerId, @Name, @Description, @StartDate, @EndDate)
              RETURNING ""Id""",
            new
            {
                entity.OwnerId,
                entity.Name,
                entity.Description,
                entity.StartDate,
                entity.EndDate
            });
    }

    public async Task<bool> UpdateAsync(Project entity)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"UPDATE ""Projects""
              SET ""Name"" = @Name, ""Description"" = @Description,
                  ""StartDate"" = @StartDate, ""EndDate"" = @EndDate
              WHERE ""Id"" = @Id",
            new
            {
                entity.Id,
                entity.Name,
                entity.Description,
                entity.StartDate,
                entity.EndDate
            });
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"DELETE FROM ""Projects""
              WHERE ""Id"" = @Id",
            new { Id = id });
        return affected > 0;
    }

    public async Task<IEnumerable<Project>> GetByOwnerIdAsync(int ownerId)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryAsync<Project>(
            @"SELECT ""Id"", ""OwnerId"", ""Name"", ""Description"", ""StartDate"", ""EndDate""
              FROM ""Projects""
              WHERE ""OwnerId"" = @OwnerId
              ORDER BY ""StartDate"" DESC",
            new { OwnerId = ownerId });
    }
}





