using backend.Application.Interfaces;
using backend.Models;
using backend.Repositories;
using Dapper;

namespace backend.Application.Repositories;

/// <summary>
/// Write-only repository for Task entity operations (CQRS - Commands)
/// </summary>
public class TaskRepository : BaseRepository, ITaskRepository
{
    public TaskRepository(string connectionString) : base(connectionString) { }

    public async Task<int> CreateAsync(TaskModel entity)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QuerySingleAsync<int>(
            @"INSERT INTO ""Tasks"" (""OwnerId"", ""StatusId"", ""CategoryId"", ""ProjectId"",
                                    ""Title"", ""Description"", ""Priority"", ""Deadline"",
                                    ""CreatedAt"", ""UpdatedAt"", ""EstimatedHours"", ""ActualHours"")
              VALUES (@OwnerId, @StatusId, @CategoryId, @ProjectId, @Title, @Description,
                      @Priority, @Deadline, @CreatedAt, @UpdatedAt, @EstimatedHours, @ActualHours)
              RETURNING ""Id""",
            new
            {
                entity.OwnerId,
                entity.StatusId,
                entity.CategoryId,
                entity.ProjectId,
                entity.Title,
                entity.Description,
                entity.Priority,
                entity.Deadline,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.EstimatedHours,
                entity.ActualHours
            });
    }

    public async Task<bool> UpdateAsync(TaskModel entity)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"UPDATE ""Tasks""
              SET ""StatusId"" = @StatusId, ""CategoryId"" = @CategoryId, ""ProjectId"" = @ProjectId,
                  ""Title"" = @Title, ""Description"" = @Description, ""Priority"" = @Priority,
                  ""Deadline"" = @Deadline, ""UpdatedAt"" = @UpdatedAt,
                  ""EstimatedHours"" = @EstimatedHours, ""ActualHours"" = @ActualHours
              WHERE ""Id"" = @Id",
            new
            {
                entity.Id,
                entity.StatusId,
                entity.CategoryId,
                entity.ProjectId,
                entity.Title,
                entity.Description,
                entity.Priority,
                entity.Deadline,
                entity.UpdatedAt,
                entity.EstimatedHours,
                entity.ActualHours
            });
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"DELETE FROM ""Tasks""
              WHERE ""Id"" = @Id",
            new { Id = id });
        return affected > 0;
    }
}

