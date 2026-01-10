using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using Dapper;

namespace backend.Infrastructure.Repositories;

/// <summary>
/// Repository for User entity operations
/// </summary>
public class UserRepository : BaseRepository, IUserRepository
{
    public UserRepository(string connectionString) : base(connectionString) { }

    public async Task<User?> GetByIdAsync(int id)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<User>(
            @"SELECT ""Id"", ""Name"", ""Surname"", ""Email"", ""Login"", ""PasswordHash"", 
                     ""CreatedAt"", ""LastLoginAt"", ""IsActive"", ""EmailConfirmed""
              FROM ""Users""
              WHERE ""Id"" = @Id",
            new { Id = id });
    }

    public async Task<User?> GetByLoginOrEmailAsync(string loginOrEmail)
    {
        var normalized = loginOrEmail.Trim();

        await using var connection = await GetConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<User>(
            """
            SELECT
                "Id",
                "Name",
                "Surname",
                "Email",
                "Login",
                "PasswordHash",
                "CreatedAt",
                "LastLoginAt",
                "IsActive",
                "EmailConfirmed"
            FROM "Users"
            WHERE "IsActive" = true
              AND (
                  LOWER("Email") = @Value
                  OR LOWER("Login") = @Value
              )
            LIMIT 1
            """,
            new { Value = normalized }
        );
    }

    public async Task<(bool loginExists, bool emailExists)> CheckUserExistsAsync(string login, string email)
    {
        await using var connection = await GetConnectionAsync();

        var result = await connection.QuerySingleAsync<dynamic>(
            """
            SELECT 
                EXISTS(SELECT 1 FROM "Users" WHERE LOWER("Login") = LOWER(@Login)) AS LoginExists,
                EXISTS(SELECT 1 FROM "Users" WHERE LOWER("Email") = LOWER(@Email)) AS EmailExists
            """,
            new { Login = login, Email = email });

        return ((bool)result.loginexists, (bool)result.emailexists);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryAsync<User>(
            @"SELECT ""Id"", ""Name"", ""Surname"", ""Email"", ""Login"", ""PasswordHash"", 
                     ""CreatedAt"", ""LastLoginAt"", ""IsActive"", ""EmailConfirmed""
              FROM ""Users""
              ORDER BY ""CreatedAt"" DESC");
    }

    public async Task<int> CreateAsync(User user)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QuerySingleAsync<int>(
            @"INSERT INTO ""Users"" (""Name"", ""Surname"", ""Email"", ""Login"", ""PasswordHash"", 
                                     ""CreatedAt"", ""LastLoginAt"", ""IsActive"", ""EmailConfirmed"")
              VALUES (@Name, @Surname, @Email, @Login, @PasswordHash, 
                      @CreatedAt, @LastLoginAt, @IsActive, @EmailConfirmed)
              RETURNING ""Id""",
            new
            {
                user.Name,
                user.Surname,
                user.Email,
                user.Login,
                user.PasswordHash,
                user.CreatedAt,
                user.LastLoginAt,
                user.IsActive,
                user.EmailConfirmed
            });
    }

    public async Task<bool> UpdateAsync(User user)
    {
        await using var connection = await GetConnectionAsync();
        var affected = await connection.ExecuteAsync(
            @"UPDATE ""Users""
              SET ""Name"" = @Name, ""Surname"" = @Surname, ""Email"" = @Email, ""Login"" = @Login,
                  ""PasswordHash"" = @PasswordHash, ""LastLoginAt"" = @LastLoginAt,
                  ""IsActive"" = @IsActive, ""EmailConfirmed"" = @EmailConfirmed
              WHERE ""Id"" = @Id",
            new
            {
                user.Id,
                user.Name,
                user.Surname,
                user.Email,
                user.Login,
                user.PasswordHash,
                user.LastLoginAt,
                user.IsActive,
                user.EmailConfirmed
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
