using backend.Interfaces;
using backend.Models;
using Dapper;

namespace backend.Repositories;

/// <summary>
/// Repository for Category entity operations
/// </summary>
public class CategoryRepository : BaseRepository, IRepository<Category>
{
    public CategoryRepository(string connectionString) : base(connectionString) { }

    public async Task<Category?> GetByIdAsync(int id)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<Category>(
            @"SELECT ""Id"", ""Name"", ""Color""
              FROM ""Categories""
              WHERE ""Id"" = @Id",
            new { Id = id });
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryAsync<Category>(
            @"SELECT ""Id"", ""Name"", ""Color""
              FROM ""Categories""
              ORDER BY ""Name""");
    }

    public async Task<int> CreateAsync(Category entity)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QuerySingleAsync<int>(
            @"INSERT INTO ""Categories"" (""Name"", ""Color"")
              VALUES (@Name, @Color)
              RETURNING ""Id""",
            new { entity.Name, entity.Color });
    }

    public async Task<bool> UpdateAsync(Category entity)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"UPDATE ""Categories""
              SET ""Name"" = @Name, ""Color"" = @Color
              WHERE ""Id"" = @Id",
            new { entity.Id, entity.Name, entity.Color });
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"DELETE FROM ""Categories""
              WHERE ""Id"" = @Id",
            new { Id = id });
        return affected > 0;
    }
}





