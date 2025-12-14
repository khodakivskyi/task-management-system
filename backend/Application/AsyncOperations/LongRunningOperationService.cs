using System.Diagnostics;
using Npgsql;

namespace backend.Application.AsyncOperations;

/// <summary>
/// Service demonstrating async operations with CancellationToken support
/// </summary>
public class LongRunningOperationService
{
    private readonly string _connectionString;

    public LongRunningOperationService(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    /// <summary>
    /// Executes a long-running SQL operation using pg_sleep (PostgreSQL equivalent of WAITFOR DELAY)
    /// Demonstrates: CancellationToken support in all async methods
    /// </summary>
    public async Task<int> ExecuteLongRunningOperationAsync(
        int sleepSeconds,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        // Open connection with cancellation token support
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        // Create command with cancellation token support
        await using var command = new NpgsqlCommand($"SELECT pg_sleep({sleepSeconds})", connection);

        try
        {
            // Execute command with cancellation token
            await command.ExecuteNonQueryAsync(cancellationToken);
            
            stopwatch.Stop();
            Console.WriteLine($"✓ Long operation completed in {stopwatch.ElapsedMilliseconds} ms");
            return (int)stopwatch.ElapsedMilliseconds;
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            Console.WriteLine($"✗ Operation was cancelled after {stopwatch.ElapsedMilliseconds} ms");
            throw;
        }
    }

    /// <summary>
    /// Executes a long-running query that reads data
    /// Demonstrates: CancellationToken in ExecuteReaderAsync and ReadAsync
    /// </summary>
    public async Task<List<string>> ExecuteLongRunningQueryAsync(
        int sleepSeconds,
        CancellationToken cancellationToken = default)
    {
        var results = new List<string>();
        var stopwatch = Stopwatch.StartNew();

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        // Query that includes a sleep and returns data
        await using var command = new NpgsqlCommand(
            $@"
                SELECT 
                    'Task ' || generate_series(1, 10) as task_name,
                    pg_sleep({sleepSeconds}) as wait_time
            ",
            connection);

        try
        {
            // Execute reader with cancellation token
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            // Read data with cancellation token
            while (await reader.ReadAsync(cancellationToken))
            {
                var taskName = reader.GetString(0);
                results.Add(taskName);
            }

            stopwatch.Stop();
            Console.WriteLine($"✓ Query completed: {results.Count} rows in {stopwatch.ElapsedMilliseconds} ms");
            return results;
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            Console.WriteLine($"✗ Query was cancelled after {stopwatch.ElapsedMilliseconds} ms");
            throw;
        }
    }

    /// <summary>
    /// Executes multiple queries in parallel using Task.WhenAll
    /// Demonstrates: Parallel execution and handling failures
    /// </summary>
    public async Task<ParallelExecutionResult> ExecuteParallelQueriesAsync(
        int numberOfQueries,
        int sleepSecondsPerQuery,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var tasks = new List<Task<int>>();
        var results = new ParallelExecutionResult
        {
            TotalQueries = numberOfQueries,
            SuccessfulQueries = 0,
            FailedQueries = 0,
            Errors = new List<string>()
        };

        // Create multiple tasks
        for (int i = 0; i < numberOfQueries; i++)
        {
            int queryIndex = i + 1;
            tasks.Add(ExecuteSingleQueryAsync(queryIndex, sleepSecondsPerQuery, cancellationToken));
        }

        try
        {
            // Execute all queries in parallel
            var completedTasks = await Task.WhenAll(tasks);
            
            results.SuccessfulQueries = completedTasks.Length;
            results.TotalExecutionTime = stopwatch.ElapsedMilliseconds;
            
            stopwatch.Stop();
            Console.WriteLine($"✓ All {numberOfQueries} queries completed in {stopwatch.ElapsedMilliseconds} ms");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            results.TotalExecutionTime = stopwatch.ElapsedMilliseconds;

            // Count successful and failed tasks
            foreach (var task in tasks)
            {
                if (task.IsCompletedSuccessfully)
                {
                    results.SuccessfulQueries++;
                }
                else if (task.IsFaulted)
                {
                    results.FailedQueries++;
                    results.Errors.Add($"Query failed: {task.Exception?.GetBaseException().Message ?? "Unknown error"}");
                }
                else if (task.IsCanceled)
                {
                    results.FailedQueries++;
                    results.Errors.Add("Query was cancelled");
                }
            }

            Console.WriteLine($"⚠ Parallel execution completed with errors: {results.SuccessfulQueries} succeeded, {results.FailedQueries} failed");
        }

        return results;
    }

    /// <summary>
    /// Helper method to execute a single query
    /// </summary>
    private async Task<int> ExecuteSingleQueryAsync(
        int queryIndex,
        int sleepSeconds,
        CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        // Simulate different queries with different sleep times
        var sleepTime = sleepSeconds + (queryIndex % 2); // Vary sleep time slightly
        
        await using var command = new NpgsqlCommand(
            $"SELECT pg_sleep({sleepTime}), 'Query {queryIndex}' as query_name",
            connection);

        // Simulate occasional failure (every 3rd query fails)
        if (queryIndex % 3 == 0)
        {
            throw new InvalidOperationException($"Simulated failure for query {queryIndex}");
        }

        await command.ExecuteNonQueryAsync(cancellationToken);
        return queryIndex;
    }
}

/// <summary>
/// Result of parallel query execution
/// </summary>
public class ParallelExecutionResult
{
    public int TotalQueries { get; set; }
    public int SuccessfulQueries { get; set; }
    public int FailedQueries { get; set; }
    public long TotalExecutionTime { get; set; }
    public List<string> Errors { get; set; } = new();
}

