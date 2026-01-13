using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using Dapper;

namespace backend.Infrastructure.Repositories;

/// <summary>
/// Repository for Category entity operations (read-only)
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
}





