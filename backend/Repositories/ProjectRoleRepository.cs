using backend.Interfaces;
using backend.Models;
using Dapper;

namespace backend.Repositories;

/// <summary>
/// Repository for ProjectRole entity operations
/// </summary>
public class ProjectRoleRepository : BaseRepository, IRepository<ProjectRole>
{
    public ProjectRoleRepository(string connectionString) : base(connectionString) { }

    public async Task<ProjectRole?> GetByIdAsync(int id)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<ProjectRole>(
            @"SELECT ""Id"", ""Name"", ""CanCreateTasks"", ""CanEditTasks"", ""CanDeleteTasks"",
                     ""CanAssignTasks"", ""CanManageMembers""
              FROM ""ProjectRoles""
              WHERE ""Id"" = @Id",
            new { Id = id });
    }

    public async Task<IEnumerable<ProjectRole>> GetAllAsync()
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryAsync<ProjectRole>(
            @"SELECT ""Id"", ""Name"", ""CanCreateTasks"", ""CanEditTasks"", ""CanDeleteTasks"",
                     ""CanAssignTasks"", ""CanManageMembers""
              FROM ""ProjectRoles""
              ORDER BY ""Name""");
    }

    public async Task<int> CreateAsync(ProjectRole entity)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QuerySingleAsync<int>(
            @"INSERT INTO ""ProjectRoles"" (""Name"", ""CanCreateTasks"", ""CanEditTasks"",
                                           ""CanDeleteTasks"", ""CanAssignTasks"", ""CanManageMembers"")
              VALUES (@Name, @CanCreateTasks, @CanEditTasks, @CanDeleteTasks, @CanAssignTasks, @CanManageMembers)
              RETURNING ""Id""",
            new
            {
                entity.Name,
                entity.CanCreateTasks,
                entity.CanEditTasks,
                entity.CanDeleteTasks,
                entity.CanAssignTasks,
                entity.CanManageMembers
            });
    }

    public async Task<bool> UpdateAsync(ProjectRole entity)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"UPDATE ""ProjectRoles""
              SET ""Name"" = @Name, ""CanCreateTasks"" = @CanCreateTasks,
                  ""CanEditTasks"" = @CanEditTasks, ""CanDeleteTasks"" = @CanDeleteTasks,
                  ""CanAssignTasks"" = @CanAssignTasks, ""CanManageMembers"" = @CanManageMembers
              WHERE ""Id"" = @Id",
            new
            {
                entity.Id,
                entity.Name,
                entity.CanCreateTasks,
                entity.CanEditTasks,
                entity.CanDeleteTasks,
                entity.CanAssignTasks,
                entity.CanManageMembers
            });
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"DELETE FROM ""ProjectRoles""
              WHERE ""Id"" = @Id",
            new { Id = id });
        return affected > 0;
    }
}





