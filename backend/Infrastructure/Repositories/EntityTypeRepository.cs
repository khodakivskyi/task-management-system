using backend.Exceptions;
using backend.Interfaces;
using backend.Models;
using Dapper;

namespace backend.Infrastructure.Repositories;

/// <summary>
/// Repository for EntityType entity operations (read-only)
/// EntityTypes are default values in the database and cannot be created, updated, or deleted
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
        throw new ForbiddenException("EntityTypes are default values and cannot be created. They must be defined in the database.");
    }

    public async Task<bool> UpdateAsync(EntityType entity)
    {
        throw new ForbiddenException("EntityTypes are default values and cannot be updated. They must be modified in the database.");
    }

    public async Task<bool> DeleteAsync(int id)
    {
        throw new ForbiddenException("EntityTypes are default values and cannot be deleted. They must be removed from the database.");
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





