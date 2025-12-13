using backend.Models;
using backend.Repositories;
using Dapper;

namespace backend.Application.QueryRepositories;

/// <summary>
/// Read-only repository for Task queries (CQRS - Queries)
/// </summary>
public class TaskQueryRepository : BaseRepository
{
    public TaskQueryRepository(string connectionString) : base(connectionString) { }

    public async Task<TaskModel?> GetByIdAsync(int id)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<TaskModel>(
            @"SELECT ""Id"", ""OwnerId"", ""StatusId"", ""CategoryId"", ""ProjectId"",
                     ""Title"", ""Description"", ""Priority"", ""Deadline"",
                     ""CreatedAt"", ""UpdatedAt"", ""EstimatedHours"", ""ActualHours""
              FROM ""Tasks""
              WHERE ""Id"" = @Id",
            new { Id = id });
    }

    public async Task<IEnumerable<TaskModel>> GetAllAsync()
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryAsync<TaskModel>(
            @"SELECT ""Id"", ""OwnerId"", ""StatusId"", ""CategoryId"", ""ProjectId"",
                     ""Title"", ""Description"", ""Priority"", ""Deadline"",
                     ""CreatedAt"", ""UpdatedAt"", ""EstimatedHours"", ""ActualHours""
              FROM ""Tasks""
              ORDER BY ""CreatedAt"" DESC");
    }

    public async Task<IEnumerable<TaskModel>> GetByProjectIdAsync(int projectId)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryAsync<TaskModel>(
            @"SELECT ""Id"", ""OwnerId"", ""StatusId"", ""CategoryId"", ""ProjectId"",
                     ""Title"", ""Description"", ""Priority"", ""Deadline"",
                     ""CreatedAt"", ""UpdatedAt"", ""EstimatedHours"", ""ActualHours""
              FROM ""Tasks""
              WHERE ""ProjectId"" = @ProjectId
              ORDER BY ""CreatedAt"" DESC",
            new { ProjectId = projectId });
    }

    public async Task<IEnumerable<TaskModel>> GetByOwnerIdAsync(int ownerId)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryAsync<TaskModel>(
            @"SELECT ""Id"", ""OwnerId"", ""StatusId"", ""CategoryId"", ""ProjectId"",
                     ""Title"", ""Description"", ""Priority"", ""Deadline"",
                     ""CreatedAt"", ""UpdatedAt"", ""EstimatedHours"", ""ActualHours""
              FROM ""Tasks""
              WHERE ""OwnerId"" = @OwnerId
              ORDER BY ""CreatedAt"" DESC",
            new { OwnerId = ownerId });
    }

    /// <summary>
    /// Gets paginated tasks with joined data from related tables
    /// </summary>
    public async Task<IEnumerable<TaskWithDetailsDto>> GetPagedAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string sortBy = "CreatedAt",
        string sortDirection = "DESC",
        string? filterValue = null)
    {
        if (pageNumber < 1) throw new ArgumentException("Page number must be >= 1");
        if (pageSize < 1) throw new ArgumentException("Page size must be >= 1");

        // Validate and map sortBy column
        var sortColumnMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "t.\"Id\"" },
            { "Title", "t.\"Title\"" },
            { "Priority", "t.\"Priority\"" },
            { "Deadline", "t.\"Deadline\"" },
            { "CreatedAt", "t.\"CreatedAt\"" },
            { "UpdatedAt", "t.\"UpdatedAt\"" },
            { "EstimatedHours", "t.\"EstimatedHours\"" },
            { "ActualHours", "t.\"ActualHours\"" },
            { "StatusName", "s.\"Name\"" },
            { "CategoryName", "c.\"Name\"" },
            { "OwnerName", "u.\"Name\"" }
        };

        var orderByColumn = sortColumnMap.ContainsKey(sortBy) 
            ? sortColumnMap[sortBy] 
            : "t.\"CreatedAt\"";

        sortDirection = sortDirection.ToUpper() == "ASC" ? "ASC" : "DESC";

        // Build WHERE clause
        var whereClause = string.IsNullOrWhiteSpace(filterValue)
            ? ""
            : @"WHERE (t.""Title"" ILIKE @FilterValue 
                   OR t.""Description"" ILIKE @FilterValue
                   OR s.""Name"" ILIKE @FilterValue
                   OR c.""Name"" ILIKE @FilterValue
                   OR u.""Name"" ILIKE @FilterValue
                   OR u.""Surname"" ILIKE @FilterValue)";

        string sql = $@"
            SELECT 
                t.""Id"",
                t.""OwnerId"",
                t.""StatusId"",
                t.""CategoryId"",
                t.""ProjectId"",
                t.""Title"",
                t.""Description"",
                t.""Priority"",
                t.""Deadline"",
                t.""CreatedAt"",
                t.""UpdatedAt"",
                t.""EstimatedHours"",
                t.""ActualHours"",
                s.""Name"" AS ""StatusName"",
                s.""Color"" AS ""StatusColor"",
                c.""Name"" AS ""CategoryName"",
                c.""Color"" AS ""CategoryColor"",
                u.""Name"" AS ""OwnerName"",
                u.""Surname"" AS ""OwnerSurname"",
                u.""Login"" AS ""OwnerLogin""
            FROM ""Tasks"" t
            INNER JOIN ""Statuses"" s ON t.""StatusId"" = s.""Id""
            LEFT JOIN ""Categories"" c ON t.""CategoryId"" = c.""Id""
            INNER JOIN ""Users"" u ON t.""OwnerId"" = u.""Id""
            {whereClause}
            ORDER BY {orderByColumn} {sortDirection}
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY";

        await using var connection = await GetConnectionAsync();
        return await connection.QueryAsync<TaskWithDetailsDto>(sql, new
        {
            Offset = (pageNumber - 1) * pageSize,
            PageSize = pageSize,
            FilterValue = string.IsNullOrWhiteSpace(filterValue) ? null : $"%{filterValue}%"
        });
    }
}

