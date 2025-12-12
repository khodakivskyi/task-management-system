using backend.Interfaces;
using backend.Models;
using Dapper;

namespace backend.Repositories;

/// <summary>
/// Repository for EntityType entity operations
/// </summary>
public class EntityTypeRepository : BaseRepository, IRepository<EntityType>
{
    public EntityTypeRepository(string connectionString) : base(connectionString) { }

    public async Task<EntityType?> GetByIdAsync(int id)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<EntityType>(
            @"SELECT ""Id"", ""Name""
              FROM ""EntityTypes""
              WHERE ""Id"" = @Id",
            new { Id = id });
    }

    public async Task<IEnumerable<EntityType>> GetAllAsync()
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryAsync<EntityType>(
            @"SELECT ""Id"", ""Name""
              FROM ""EntityTypes""
              ORDER BY ""Name""");
    }

    public async Task<int> CreateAsync(EntityType entity)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QuerySingleAsync<int>(
            @"INSERT INTO ""EntityTypes"" (""Name"")
              VALUES (@Name)
              RETURNING ""Id""",
            new { entity.Name });
    }

    public async Task<bool> UpdateAsync(EntityType entity)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"UPDATE ""EntityTypes""
              SET ""Name"" = @Name
              WHERE ""Id"" = @Id",
            new { entity.Id, entity.Name });
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"DELETE FROM ""EntityTypes""
              WHERE ""Id"" = @Id",
            new { Id = id });
        return affected > 0;
    }

    public async Task<EntityType?> GetByNameAsync(string name)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<EntityType>(
            @"SELECT ""Id"", ""Name""
              FROM ""EntityTypes""
              WHERE ""Name"" = @Name",
            new { Name = name });
    }
}





