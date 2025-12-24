using backend.Interfaces;
using backend.Models;
using Dapper;
using Npgsql;

namespace backend.Repositories;

/// <summary>
/// Repository for Comment entity operations
/// </summary>
public class CommentRepository : BaseRepository, IRepository<Comment>
{
    public CommentRepository(string connectionString) : base(connectionString) { }

    public async Task<Comment?> GetByIdAsync(int id)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<Comment>(
            @"SELECT ""Id"", ""TaskId"", ""UserId"", ""Content"", ""CreatedAt""
              FROM ""Comments""
              WHERE ""Id"" = @Id",
            new { Id = id });
    }

    public async Task<IEnumerable<Comment>> GetAllAsync()
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryAsync<Comment>(
            @"SELECT ""Id"", ""TaskId"", ""UserId"", ""Content"", ""CreatedAt""
              FROM ""Comments""
              ORDER BY ""CreatedAt"" DESC");
    }

    public async Task<int> CreateAsync(Comment entity)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QuerySingleAsync<int>(
            @"INSERT INTO ""Comments"" (""TaskId"", ""UserId"", ""Content"", ""CreatedAt"")
              VALUES (@TaskId, @UserId, @Content, @CreatedAt)
              RETURNING ""Id""",
            new
            {
                entity.TaskId,
                entity.UserId,
                entity.Content,
                entity.CreatedAt
            });
    }

    public async Task<bool> UpdateAsync(Comment entity)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"UPDATE ""Comments""
              SET ""Content"" = @Content
              WHERE ""Id"" = @Id",
            new { entity.Id, entity.Content });
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"DELETE FROM ""Comments""
              WHERE ""Id"" = @Id",
            new { Id = id });
        return affected > 0;
    }

    public async Task<IEnumerable<Comment>> GetByTaskIdAsync(int taskId)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryAsync<Comment>(
            @"SELECT ""Id"", ""TaskId"", ""UserId"", ""Content"", ""CreatedAt""
              FROM ""Comments""
              WHERE ""TaskId"" = @TaskId
              ORDER BY ""CreatedAt"" ASC",
            new { TaskId = taskId });
    }

    public async Task<bool> DeleteByTaskIdAsync(int taskId, NpgsqlTransaction? transaction = null)
    {
        var connection = transaction?.Connection ?? await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"DELETE FROM ""Comments""
              WHERE ""TaskId"" = @TaskId",
            new { TaskId = taskId },
            transaction);
        return affected > 0;
    }
}





