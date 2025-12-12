using backend.Interfaces;
using backend.Models;
using Dapper;

namespace backend.Repositories;

/// <summary>
/// Repository for Favorite entity operations
/// </summary>
public class FavoriteRepository : BaseRepository, IRepository<Favorite>
{
    public FavoriteRepository(string connectionString) : base(connectionString) { }

    public async Task<Favorite?> GetByIdAsync(int id)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<Favorite>(
            @"SELECT ""Id"", ""UserId"", ""EntityTypeId"", ""EntityId"", ""CreatedAt""
              FROM ""Favorites""
              WHERE ""Id"" = @Id",
            new { Id = id });
    }

    public async Task<IEnumerable<Favorite>> GetAllAsync()
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryAsync<Favorite>(
            @"SELECT ""Id"", ""UserId"", ""EntityTypeId"", ""EntityId"", ""CreatedAt""
              FROM ""Favorites""
              ORDER BY ""CreatedAt"" DESC");
    }

    public async Task<int> CreateAsync(Favorite entity)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QuerySingleAsync<int>(
            @"INSERT INTO ""Favorites"" (""UserId"", ""EntityTypeId"", ""EntityId"", ""CreatedAt"")
              VALUES (@UserId, @EntityTypeId, @EntityId, @CreatedAt)
              RETURNING ""Id""",
            new
            {
                entity.UserId,
                entity.EntityTypeId,
                entity.EntityId,
                entity.CreatedAt
            });
    }

    public async Task<bool> UpdateAsync(Favorite entity)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"UPDATE ""Favorites""
              SET ""EntityTypeId"" = @EntityTypeId, ""EntityId"" = @EntityId
              WHERE ""Id"" = @Id",
            new { entity.Id, entity.EntityTypeId, entity.EntityId });
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"DELETE FROM ""Favorites""
              WHERE ""Id"" = @Id",
            new { Id = id });
        return affected > 0;
    }

    public async Task<Favorite?> GetByUserAndEntityAsync(int userId, int entityTypeId, int entityId)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<Favorite>(
            @"SELECT ""Id"", ""UserId"", ""EntityTypeId"", ""EntityId"", ""CreatedAt""
              FROM ""Favorites""
              WHERE ""UserId"" = @UserId AND ""EntityTypeId"" = @EntityTypeId AND ""EntityId"" = @EntityId",
            new { UserId = userId, EntityTypeId = entityTypeId, EntityId = entityId });
    }

    public async Task<IEnumerable<Favorite>> GetByUserIdAsync(int userId)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryAsync<Favorite>(
            @"SELECT ""Id"", ""UserId"", ""EntityTypeId"", ""EntityId"", ""CreatedAt""
              FROM ""Favorites""
              WHERE ""UserId"" = @UserId
              ORDER BY ""CreatedAt"" DESC",
            new { UserId = userId });
    }

    public async Task<bool> DeleteByUserAndEntityAsync(int userId, int entityTypeId, int entityId)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"DELETE FROM ""Favorites""
              WHERE ""UserId"" = @UserId AND ""EntityTypeId"" = @EntityTypeId AND ""EntityId"" = @EntityId",
            new { UserId = userId, EntityTypeId = entityTypeId, EntityId = entityId });
        return affected > 0;
    }
}





