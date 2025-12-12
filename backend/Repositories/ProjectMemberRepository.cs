using backend.Interfaces;
using backend.Models;
using Dapper;

namespace backend.Repositories;

/// <summary>
/// Repository for ProjectMember entity operations
/// </summary>
public class ProjectMemberRepository : BaseRepository, IRepository<ProjectMember>
{
    public ProjectMemberRepository(string connectionString) : base(connectionString) { }

    public async Task<ProjectMember?> GetByIdAsync(int id)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<ProjectMember>(
            @"SELECT ""Id"", ""ProjectId"", ""UserId"", ""RoleId"", ""JoinedAt""
              FROM ""ProjectMembers""
              WHERE ""Id"" = @Id",
            new { Id = id });
    }

    public async Task<IEnumerable<ProjectMember>> GetAllAsync()
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryAsync<ProjectMember>(
            @"SELECT ""Id"", ""ProjectId"", ""UserId"", ""RoleId"", ""JoinedAt""
              FROM ""ProjectMembers""
              ORDER BY ""JoinedAt"" DESC");
    }

    public async Task<int> CreateAsync(ProjectMember entity)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QuerySingleAsync<int>(
            @"INSERT INTO ""ProjectMembers"" (""ProjectId"", ""UserId"", ""RoleId"", ""JoinedAt"")
              VALUES (@ProjectId, @UserId, @RoleId, @JoinedAt)
              RETURNING ""Id""",
            new
            {
                entity.ProjectId,
                entity.UserId,
                entity.RoleId,
                entity.JoinedAt
            });
    }

    public async Task<bool> UpdateAsync(ProjectMember entity)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"UPDATE ""ProjectMembers""
              SET ""RoleId"" = @RoleId
              WHERE ""Id"" = @Id",
            new { entity.Id, entity.RoleId });
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"DELETE FROM ""ProjectMembers""
              WHERE ""Id"" = @Id",
            new { Id = id });
        return affected > 0;
    }

    public async Task<ProjectMember?> GetByProjectAndUserAsync(int projectId, int userId)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<ProjectMember>(
            @"SELECT ""Id"", ""ProjectId"", ""UserId"", ""RoleId"", ""JoinedAt""
              FROM ""ProjectMembers""
              WHERE ""ProjectId"" = @ProjectId AND ""UserId"" = @UserId",
            new { ProjectId = projectId, UserId = userId });
    }

    public async Task<IEnumerable<ProjectMember>> GetByProjectIdAsync(int projectId)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryAsync<ProjectMember>(
            @"SELECT ""Id"", ""ProjectId"", ""UserId"", ""RoleId"", ""JoinedAt""
              FROM ""ProjectMembers""
              WHERE ""ProjectId"" = @ProjectId
              ORDER BY ""JoinedAt""",
            new { ProjectId = projectId });
    }
}





