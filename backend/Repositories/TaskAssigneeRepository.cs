using backend.Application.Interfaces;
using backend.Interfaces;
using backend.Models;
using Dapper;
using Npgsql;

namespace backend.Repositories;

/// <summary>
/// Repository for TaskAssignee entity operations
/// </summary>
public class TaskAssigneeRepository : BaseRepository, IRepository<TaskAssignee>, ITaskAssigneeRepository
{
    public TaskAssigneeRepository(string connectionString) : base(connectionString) { }

    public async Task<TaskAssignee?> GetByIdAsync(int id)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<TaskAssignee>(
            @"SELECT ""Id"", ""TaskId"", ""UserId""
              FROM ""TaskAssignees""
              WHERE ""Id"" = @Id",
            new { Id = id });
    }

    public async Task<IEnumerable<TaskAssignee>> GetAllAsync()
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryAsync<TaskAssignee>(
            @"SELECT ""Id"", ""TaskId"", ""UserId""
              FROM ""TaskAssignees""");
    }

    public async Task<int> CreateAsync(TaskAssignee entity)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QuerySingleAsync<int>(
            @"INSERT INTO ""TaskAssignees"" (""TaskId"", ""UserId"")
              VALUES (@TaskId, @UserId)
              RETURNING ""Id""",
            new { entity.TaskId, entity.UserId });
    }

    public async Task<bool> UpdateAsync(TaskAssignee entity)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"UPDATE ""TaskAssignees""
              SET ""TaskId"" = @TaskId, ""UserId"" = @UserId
              WHERE ""Id"" = @Id",
            new { entity.Id, entity.TaskId, entity.UserId });
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"DELETE FROM ""TaskAssignees""
              WHERE ""Id"" = @Id",
            new { Id = id });
        return affected > 0;
    }

    public async Task<bool> DeleteByTaskAndUserAsync(int taskId, int userId)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"DELETE FROM ""TaskAssignees""
              WHERE ""TaskId"" = @TaskId AND ""UserId"" = @UserId",
            new { TaskId = taskId, UserId = userId });
        return affected > 0;
    }

    public async Task<IEnumerable<TaskAssignee>> GetByTaskIdAsync(int taskId)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryAsync<TaskAssignee>(
            @"SELECT ""Id"", ""TaskId"", ""UserId""
              FROM ""TaskAssignees""
              WHERE ""TaskId"" = @TaskId",
            new { TaskId = taskId });
    }

    public async Task<IEnumerable<TaskAssignee>> GetByUserIdAsync(int userId)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryAsync<TaskAssignee>(
            @"SELECT ""Id"", ""TaskId"", ""UserId""
              FROM ""TaskAssignees""
              WHERE ""UserId"" = @UserId",
            new { UserId = userId });
    }

    public async Task<bool> DeleteByTaskIdAsync(int taskId, NpgsqlTransaction? transaction = null)
    {
        var connection = transaction?.Connection ?? await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"DELETE FROM ""TaskAssignees""
              WHERE ""TaskId"" = @TaskId",
            new { TaskId = taskId },
            transaction);
        return affected > 0;
    }
}





