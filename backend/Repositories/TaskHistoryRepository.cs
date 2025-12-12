using backend.Interfaces;
using backend.Models;
using Dapper;

namespace backend.Repositories;

/// <summary>
/// Repository for TaskHistory entity operations
/// </summary>
public class TaskHistoryRepository : BaseRepository, IRepository<TaskHistory>
{
    public TaskHistoryRepository(string connectionString) : base(connectionString) { }

    public async Task<TaskHistory?> GetByIdAsync(int id)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<TaskHistory>(
            @"SELECT ""Id"", ""TaskId"", ""UserId"", ""FieldName"", ""OldValue"", ""NewValue"", ""ChangedAt""
              FROM ""TaskHistory""
              WHERE ""Id"" = @Id",
            new { Id = id });
    }

    public async Task<IEnumerable<TaskHistory>> GetAllAsync()
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryAsync<TaskHistory>(
            @"SELECT ""Id"", ""TaskId"", ""UserId"", ""FieldName"", ""OldValue"", ""NewValue"", ""ChangedAt""
              FROM ""TaskHistory""
              ORDER BY ""ChangedAt"" DESC");
    }

    public async Task<int> CreateAsync(TaskHistory entity)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QuerySingleAsync<int>(
            @"INSERT INTO ""TaskHistory"" (""TaskId"", ""UserId"", ""FieldName"", ""OldValue"", ""NewValue"", ""ChangedAt"")
              VALUES (@TaskId, @UserId, @FieldName, @OldValue, @NewValue, @ChangedAt)
              RETURNING ""Id""",
            new
            {
                entity.TaskId,
                entity.UserId,
                entity.FieldName,
                entity.OldValue,
                entity.NewValue,
                entity.ChangedAt
            });
    }

    public async Task<bool> UpdateAsync(TaskHistory entity)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"UPDATE ""TaskHistory""
              SET ""OldValue"" = @OldValue, ""NewValue"" = @NewValue
              WHERE ""Id"" = @Id",
            new { entity.Id, entity.OldValue, entity.NewValue });
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"DELETE FROM ""TaskHistory""
              WHERE ""Id"" = @Id",
            new { Id = id });
        return affected > 0;
    }

    public async Task<IEnumerable<TaskHistory>> GetByTaskIdAsync(int taskId)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryAsync<TaskHistory>(
            @"SELECT ""Id"", ""TaskId"", ""UserId"", ""FieldName"", ""OldValue"", ""NewValue"", ""ChangedAt""
              FROM ""TaskHistory""
              WHERE ""TaskId"" = @TaskId
              ORDER BY ""ChangedAt"" DESC",
            new { TaskId = taskId });
    }
}





