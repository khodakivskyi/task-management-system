using backend.Interfaces;
using backend.Models;
using Dapper;

namespace backend.Repositories;

/// <summary>
/// Repository for User entity operations
/// </summary>
public class UserRepository : BaseRepository, IRepository<User>
{
    public UserRepository(string connectionString) : base(connectionString) { }

    public async Task<User?> GetByIdAsync(int id)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<User>(
            @"SELECT ""Id"", ""Name"", ""Surname"", ""Login"", ""PasswordHash"", ""Salt"", ""CreatedAt""
              FROM ""Users""
              WHERE ""Id"" = @Id",
            new { Id = id });
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryAsync<User>(
            @"SELECT ""Id"", ""Name"", ""Surname"", ""Login"", ""PasswordHash"", ""Salt"", ""CreatedAt""
              FROM ""Users""
              ORDER BY ""CreatedAt"" DESC");
    }

    public async Task<int> CreateAsync(User user)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QuerySingleAsync<int>(
            @"INSERT INTO ""Users"" (""Name"", ""Surname"", ""Login"", ""PasswordHash"", ""Salt"", ""CreatedAt"")
              VALUES (@Name, @Surname, @Login, @PasswordHash, @Salt, @CreatedAt)
              RETURNING ""Id""",
            new
            {
                user.Name,
                user.Surname,
                user.Login,
                user.PasswordHash,
                user.Salt,
                user.CreatedAt
            });
    }

    public async Task<bool> UpdateAsync(User user)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"UPDATE ""Users""
              SET ""Name"" = @Name, ""Surname"" = @Surname, ""Login"" = @Login,
                  ""PasswordHash"" = @PasswordHash, ""Salt"" = @Salt
              WHERE ""Id"" = @Id",
            new
            {
                user.Id,
                user.Name,
                user.Surname,
                user.Login,
                user.PasswordHash,
                user.Salt
            });
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"DELETE FROM ""Users""
              WHERE ""Id"" = @Id",
            new { Id = id });
        return affected > 0;
    }
}
