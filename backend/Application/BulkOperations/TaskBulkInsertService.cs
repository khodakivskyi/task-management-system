using System.Diagnostics;
using System.Text;
using backend.Models;
using Dapper;
using Npgsql;
using NpgsqlTypes;

namespace backend.Application.BulkOperations;

/// <summary>
/// Service for bulk insert operations comparing different approaches:
/// 1. Loop INSERT - one query per record
/// 2. Batched INSERT - grouping records (50-100 per batch)
/// 3. COPY - PostgreSQL bulk copy (analog of SqlBulkCopy)
/// </summary>
public class TaskBulkInsertService
{
    private readonly string _connectionString;

    public TaskBulkInsertService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public string GetConnectionString()
    {
        return _connectionString;
    }
    
    /// <summary>
    /// Gets available IDs from database for test data generation
    /// </summary>
    private async Task<(List<int> userIds, List<int> statusIds, List<int> categoryIds, List<int> projectIds)> GetAvailableIdsAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var userIds = (await connection.QueryAsync<int>(@"SELECT ""Id"" FROM ""Users"" ORDER BY ""Id""")).ToList();
        var statusIds = (await connection.QueryAsync<int>(@"SELECT ""Id"" FROM ""Statuses"" ORDER BY ""Id""")).ToList();
        var categoryIds = (await connection.QueryAsync<int>(@"SELECT ""Id"" FROM ""Categories"" ORDER BY ""Id""")).ToList();
        var projectIds = (await connection.QueryAsync<int>(@"SELECT ""Id"" FROM ""Projects"" ORDER BY ""Id""")).ToList();

        return (userIds, statusIds, categoryIds, projectIds);
    }

    /// <summary>
    /// Generates test data for bulk insert testing using real IDs from database
    /// </summary>
    public async Task<List<TaskModel>> GenerateTestDataAsync(int count, int? ownerId = null, int? statusId = null)
    {
        var (userIds, statusIds, categoryIds, projectIds) = await GetAvailableIdsAsync();

        // Validate that we have required data
        if (userIds.Count == 0)
        {
            throw new InvalidOperationException("No users found in database. Please create at least one user.");
        }
        if (statusIds.Count == 0)
        {
            throw new InvalidOperationException("No statuses found in database. Please create at least one status.");
        }

        // Use provided IDs or get first available
        var selectedOwnerId = ownerId ?? userIds[0];
        var selectedStatusId = statusId ?? statusIds[0];

        // Validate provided IDs exist
        if (!userIds.Contains(selectedOwnerId))
        {
            throw new ArgumentException($"User with Id {selectedOwnerId} does not exist. Available IDs: {string.Join(", ", userIds)}");
        }
        if (!statusIds.Contains(selectedStatusId))
        {
            throw new ArgumentException($"Status with Id {selectedStatusId} does not exist. Available IDs: {string.Join(", ", statusIds)}");
        }

        var random = new Random();
        var tasks = new List<TaskModel>();

        for (int i = 1; i <= count; i++)
        {
            // Randomly select from available IDs, or use null if list is empty
            var categoryId = categoryIds.Count > 0 ? categoryIds[random.Next(categoryIds.Count)] : (int?)null;
            var projectId = projectIds.Count > 0 ? projectIds[random.Next(projectIds.Count)] : (int?)null;

            tasks.Add(new TaskModel
            {
                OwnerId = selectedOwnerId,
                StatusId = selectedStatusId,
                CategoryId = categoryId,
                ProjectId = projectId,
                Title = $"Test Task {i}",
                Description = $"Description for task {i}",
                Priority = random.Next(1, 6),
                Deadline = DateTime.UtcNow.AddDays(random.Next(1, 30)),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                EstimatedHours = random.Next(1, 40),
                ActualHours = 0
            });
        }

        return tasks;
    }

    /// <summary>
    /// Approach 1: Loop INSERT - one query per record
    /// Pros: Simple, safe (each record is separate transaction)
    /// Cons: Slow for large datasets
    /// </summary>
    public async Task<BulkInsertResult> InsertWithLoopAsync(List<TaskModel> tasks)
    {
        var stopwatch = Stopwatch.StartNew();
        int insertedCount = 0;

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        foreach (var task in tasks)
        {
            try
            {
                var sql = @"INSERT INTO ""Tasks"" (""OwnerId"", ""StatusId"", ""CategoryId"", ""ProjectId"",
                                                  ""Title"", ""Description"", ""Priority"", ""Deadline"",
                                                  ""CreatedAt"", ""UpdatedAt"", ""EstimatedHours"", ""ActualHours"")
                            VALUES (@OwnerId, @StatusId, @CategoryId, @ProjectId, @Title, @Description,
                                    @Priority, @Deadline, @CreatedAt, @UpdatedAt, @EstimatedHours, @ActualHours)";

                await connection.ExecuteAsync(sql, new
                {
                    task.OwnerId,
                    task.StatusId,
                    task.CategoryId,
                    task.ProjectId,
                    task.Title,
                    task.Description,
                    task.Priority,
                    task.Deadline,
                    task.CreatedAt,
                    task.UpdatedAt,
                    task.EstimatedHours,
                    task.ActualHours
                });

                insertedCount++;
            }
            catch (Exception ex)
            {
                // Log error but continue
                Console.WriteLine($"Error inserting task: {ex.Message}");
            }
        }

        stopwatch.Stop();

        return new BulkInsertResult
        {
            Method = "Loop INSERT",
            TotalRecords = tasks.Count,
            InsertedRecords = insertedCount,
            ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
            RecordsPerSecond = insertedCount / (stopwatch.ElapsedMilliseconds / 1000.0)
        };
    }

    /// <summary>
    /// Approach 2: Batched INSERT - grouping records (50-100 per batch)
    /// Pros: Faster than loop, still safe
    /// Cons: More complex, need to handle batch failures
    /// </summary>
    public async Task<BulkInsertResult> InsertWithBatchesAsync(List<TaskModel> tasks, int batchSize = 50)
    {
        var stopwatch = Stopwatch.StartNew();
        int insertedCount = 0;

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        for (int i = 0; i < tasks.Count; i += batchSize)
        {
            var batch = tasks.Skip(i).Take(batchSize).ToList();
            
            try
            {
                // Build SQL with multiple VALUES
                var sql = new StringBuilder();
                sql.Append(@"INSERT INTO ""Tasks"" (""OwnerId"", ""StatusId"", ""CategoryId"", ""ProjectId"",
                                                   ""Title"", ""Description"", ""Priority"", ""Deadline"",
                                                   ""CreatedAt"", ""UpdatedAt"", ""EstimatedHours"", ""ActualHours"")
                            VALUES ");

                var valueParts = new List<string>();
                var parameters = new DynamicParameters();

                for (int j = 0; j < batch.Count; j++)
                {
                    var task = batch[j];
                    var paramPrefix = $"p{i}_{j}_";

                    valueParts.Add($"(@{paramPrefix}OwnerId, @{paramPrefix}StatusId, @{paramPrefix}CategoryId, @{paramPrefix}ProjectId, " +
                                  $"@{paramPrefix}Title, @{paramPrefix}Description, @{paramPrefix}Priority, @{paramPrefix}Deadline, " +
                                  $"@{paramPrefix}CreatedAt, @{paramPrefix}UpdatedAt, @{paramPrefix}EstimatedHours, @{paramPrefix}ActualHours)");

                    parameters.Add($"{paramPrefix}OwnerId", task.OwnerId);
                    parameters.Add($"{paramPrefix}StatusId", task.StatusId);
                    parameters.Add($"{paramPrefix}CategoryId", task.CategoryId);
                    parameters.Add($"{paramPrefix}ProjectId", task.ProjectId);
                    parameters.Add($"{paramPrefix}Title", task.Title);
                    parameters.Add($"{paramPrefix}Description", task.Description);
                    parameters.Add($"{paramPrefix}Priority", task.Priority);
                    parameters.Add($"{paramPrefix}Deadline", task.Deadline);
                    parameters.Add($"{paramPrefix}CreatedAt", task.CreatedAt);
                    parameters.Add($"{paramPrefix}UpdatedAt", task.UpdatedAt);
                    parameters.Add($"{paramPrefix}EstimatedHours", task.EstimatedHours);
                    parameters.Add($"{paramPrefix}ActualHours", task.ActualHours);
                }

                sql.Append(string.Join(", ", valueParts));

                var affected = await connection.ExecuteAsync(sql.ToString(), parameters);
                insertedCount += affected;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting batch: {ex.Message}");
            }
        }

        stopwatch.Stop();

        return new BulkInsertResult
        {
            Method = $"Batched INSERT (batch size: {batchSize})",
            TotalRecords = tasks.Count,
            InsertedRecords = insertedCount,
            ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
            RecordsPerSecond = insertedCount / (stopwatch.ElapsedMilliseconds / 1000.0)
        };
    }

    /// <summary>
    /// Approach 3: COPY - PostgreSQL bulk copy (analog of SqlBulkCopy)
    /// Pros: Fastest method for large datasets
    /// Cons: Less error handling per record, requires binary format
    /// </summary>
    public async Task<BulkInsertResult> InsertWithCopyAsync(List<TaskModel> tasks, int batchSize = 1000)
    {
        var stopwatch = Stopwatch.StartNew();
        int insertedCount = 0;

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        try
        {
            // Use COPY for bulk insert (PostgreSQL equivalent of SqlBulkCopy)
            await using var writer = await connection.BeginBinaryImportAsync(
                @"COPY ""Tasks"" (""OwnerId"", ""StatusId"", ""CategoryId"", ""ProjectId"",
                                 ""Title"", ""Description"", ""Priority"", ""Deadline"",
                                 ""CreatedAt"", ""UpdatedAt"", ""EstimatedHours"", ""ActualHours"")
                  FROM STDIN (FORMAT BINARY)");

            foreach (var task in tasks)
            {
                await writer.StartRowAsync();
                await writer.WriteAsync(task.OwnerId);
                await writer.WriteAsync(task.StatusId);
                await writer.WriteAsync(task.CategoryId, NpgsqlDbType.Integer);
                await writer.WriteAsync(task.ProjectId, NpgsqlDbType.Integer);
                await writer.WriteAsync(task.Title);
                await writer.WriteAsync(task.Description, NpgsqlDbType.Varchar);
                await writer.WriteAsync(task.Priority, NpgsqlDbType.Integer);
                await writer.WriteAsync(task.Deadline, NpgsqlDbType.Timestamp);
                await writer.WriteAsync(task.CreatedAt);
                await writer.WriteAsync(task.UpdatedAt);
                await writer.WriteAsync(task.EstimatedHours);
                await writer.WriteAsync(task.ActualHours);
                insertedCount++;
            }

            await writer.CompleteAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error with COPY: {ex.Message}");
        }

        stopwatch.Stop();

        return new BulkInsertResult
        {
            Method = $"COPY (batch size: {batchSize})",
            TotalRecords = tasks.Count,
            InsertedRecords = insertedCount,
            ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
            RecordsPerSecond = insertedCount / (stopwatch.ElapsedMilliseconds / 1000.0)
        };
    }

    /// <summary>
    /// Compares all three bulk insert methods and measures performance
    /// </summary>
    public async Task<ComparisonResult> CompareAllMethodsAsync(int recordCount, int? ownerId = null, int? statusId = null)
    {
        var testData = await GenerateTestDataAsync(recordCount, ownerId, statusId);
        var results = new List<BulkInsertResult>();

        // Test 1: Loop INSERT
        var loopResult = await InsertWithLoopAsync(new List<TaskModel>(testData));
        results.Add(loopResult);
        await ClearTestTasksAsync();

        // Test 2: Batched INSERT (50)
        var batch50Result = await InsertWithBatchesAsync(new List<TaskModel>(testData), 50);
        results.Add(batch50Result);
        await ClearTestTasksAsync();

        // Test 3: Batched INSERT (100)
        var batch100Result = await InsertWithBatchesAsync(new List<TaskModel>(testData), 100);
        results.Add(batch100Result);
        await ClearTestTasksAsync();

        // Test 4: COPY
        var copyResult = await InsertWithCopyAsync(new List<TaskModel>(testData), 1000);
        results.Add(copyResult);

        return new ComparisonResult
        {
            RecordCount = recordCount,
            Results = results
        };
    }

    /// <summary>
    /// Clears test tasks from database
    /// </summary>
    private async Task ClearTestTasksAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        await connection.ExecuteAsync(@"DELETE FROM ""Tasks"" WHERE ""Title"" LIKE 'Test Task%'");
    }
}

