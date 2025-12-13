using System.Diagnostics;
using backend.Models;
using Dapper;
using Npgsql;

namespace backend.Application.BulkOperations;

/// <summary>
/// Console-based tester for bulk operations with formatted output
/// </summary>
public class BulkOperationsTester
{
    private readonly TaskBulkInsertService _service;
    private readonly int? _ownerId;
    private readonly int? _statusId;

    public BulkOperationsTester(string connectionString, int? ownerId = null, int? statusId = null)
    {
        _service = new TaskBulkInsertService(connectionString);
        _ownerId = ownerId;
        _statusId = statusId;
    }

    /// <summary>
    /// Runs comprehensive tests and displays formatted results in console
    /// </summary>
    public async Task RunTestsAsync()
    {
        Console.WriteLine();
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("  BULK OPERATIONS PERFORMANCE TEST");
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine();

        // Test 1: Small dataset (100 records)
        await RunTestAsync(100, "Small Dataset");

        Console.WriteLine();
        Console.WriteLine();

        // Test 2: Medium dataset (1000 records)
        await RunTestAsync(1000, "Medium Dataset");

        Console.WriteLine();
        Console.WriteLine();

        // Test 3: Large dataset (10000 records)
        await RunTestAsync(10000, "Large Dataset");
    }

    private async Task RunTestAsync(int recordCount, string testName)
    {
        Console.WriteLine($"═══════════════════════════════════════════════════════════════");
        Console.WriteLine($"  TEST: {testName} Comparison ({recordCount:N0} records)");
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine();

        var testData = await _service.GenerateTestDataAsync(recordCount, _ownerId, _statusId);
        var results = new List<BulkInsertResult>();

        // Test 1: Loop INSERT
        Console.WriteLine("▶ Method 1: Individual INSERT statements...");
        var loopResult = await RunWithTimingAsync(
            () => _service.InsertWithLoopAsync(new List<TaskModel>(testData)),
            "Loop INSERT"
        );
        results.Add(loopResult);
        await ClearTestDataAsync();
        DisplayResult(loopResult);

        Console.WriteLine();

        // Test 2: Batched INSERT (50)
        Console.WriteLine("▶ Method 2: Batched INSERT statements (batch size: 50)...");
        var batch50Result = await RunWithTimingAsync(
            () => _service.InsertWithBatchesAsync(new List<TaskModel>(testData), 50),
            "Batched INSERT (50)"
        );
        results.Add(batch50Result);
        await ClearTestDataAsync();
        DisplayResult(batch50Result);

        Console.WriteLine();

        // Test 3: Batched INSERT (100)
        Console.WriteLine("▶ Method 2: Batched INSERT statements (batch size: 100)...");
        var batch100Result = await RunWithTimingAsync(
            () => _service.InsertWithBatchesAsync(new List<TaskModel>(testData), 100),
            "Batched INSERT (100)"
        );
        results.Add(batch100Result);
        await ClearTestDataAsync();
        DisplayResult(batch100Result);

        Console.WriteLine();

        // Test 4: COPY
        Console.WriteLine("▶ Method 3: COPY (PostgreSQL bulk copy)...");
        var copyResult = await RunWithTimingAsync(
            () => _service.InsertWithCopyAsync(new List<TaskModel>(testData), 1000),
            "COPY"
        );
        results.Add(copyResult);
        DisplayResult(copyResult);

        // Display comparison
        Console.WriteLine();
        DisplayComparison(results, recordCount);
    }

    private async Task<BulkInsertResult> RunWithTimingAsync(
        Func<Task<BulkInsertResult>> operation,
        string methodName)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await operation();
        stopwatch.Stop();
        return result;
    }

    private void DisplayResult(BulkInsertResult result)
    {
        var msPerRecord = result.ElapsedMilliseconds / (double)result.InsertedRecords;
        Console.WriteLine($"✓ Inserted {result.InsertedRecords:N0} tasks in {result.ElapsedMilliseconds:N0} ms ({msPerRecord:F2} ms per task)");
    }

    private void DisplayComparison(List<BulkInsertResult> results, int recordCount)
    {
        var baseline = results[0]; // Loop INSERT as baseline
        Console.WriteLine($"📊 PERFORMANCE COMPARISON ({recordCount:N0} records):");

        foreach (var result in results)
        {
            var speedup = baseline.ElapsedMilliseconds / (double)result.ElapsedMilliseconds;
            var speedupText = speedup > 1 ? $"{speedup:F1}x faster" : $"{1 / speedup:F1}x slower";
            
            Console.WriteLine($"  {result.Method.PadRight(30)} {result.ElapsedMilliseconds,8:N0} ms ({speedupText})");
        }
    }

    private async Task ClearTestDataAsync()
    {
        await using var connection = new Npgsql.NpgsqlConnection(_service.GetConnectionString());
        await connection.OpenAsync();
        await connection.ExecuteAsync(@"DELETE FROM ""Tasks"" WHERE ""Title"" LIKE 'Test Task%'");
    }
}

