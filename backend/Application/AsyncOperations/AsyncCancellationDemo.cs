using System.Diagnostics;
using System.Text;

namespace backend.Application.AsyncOperations;

/// <summary>
/// Demonstrates async operations with CancellationToken support:
/// - Automatic timeout cancellation
/// - Manual cancellation (key press)
/// - Parallel execution with Task.WhenAll
/// - Proper exception handling
/// </summary>
public class AsyncCancellationDemo
{
    private readonly LongRunningOperationService _service;

    public AsyncCancellationDemo(LongRunningOperationService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    /// <summary>
    /// Demonstrates automatic timeout cancellation
    /// </summary>
    public async Task DemonstrateTimeoutCancellationAsync()
    {
        Console.WriteLine("\n" + new string('═', 70));
        Console.WriteLine("DEMONSTRATION 1: Automatic Timeout Cancellation");
        Console.WriteLine(new string('═', 70));
        Console.WriteLine("This will attempt to run a 10-second operation with a 3-second timeout.");
        Console.WriteLine("The operation should be cancelled automatically.\n");

        // Create CancellationTokenSource with 3-second timeout
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

        try
        {
            var stopwatch = Stopwatch.StartNew();
            await _service.ExecuteLongRunningOperationAsync(10, cts.Token);
            stopwatch.Stop();
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("✓ OperationCanceledException was caught and handled correctly");
            Console.WriteLine("✓ The operation was cancelled due to timeout\n");
        }
    }

    /// <summary>
    /// Demonstrates manual cancellation via key press
    /// </summary>
    public async Task DemonstrateManualCancellationAsync()
    {
        Console.WriteLine("\n" + new string('═', 70));
        Console.WriteLine("DEMONSTRATION 2: Manual Cancellation (Press 'C' to cancel)");
        Console.WriteLine(new string('═', 70));
        Console.WriteLine("A 10-second operation will start. Press 'C' to cancel it.");
        Console.WriteLine("(If you don't press 'C', it will complete automatically)\n");

        // Create CancellationTokenSource for manual cancellation
        using var cts = new CancellationTokenSource();

        // Start the long-running operation
        var operationTask = _service.ExecuteLongRunningOperationAsync(10, cts.Token);

        // Create a separate cancellation token for key monitor that will be cancelled when operation completes
        using var keyMonitorCts = new CancellationTokenSource();
        
        // Start a task to monitor for key press
        var keyMonitorTask = Task.Run(() =>
        {
            try
            {
                // Poll for key press with cancellation support
                while (!keyMonitorCts.Token.IsCancellationRequested)
                {
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true);
                        if (key.KeyChar == 'c' || key.KeyChar == 'C')
                        {
                            Console.WriteLine("\n⚠ Cancellation requested by user...");
                            cts.Cancel();
                            break;
                        }
                    }
                    // Small delay to avoid busy waiting
                    Thread.Sleep(50);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when operation completes
            }
            catch (Exception)
            {
                // Ignore other errors in key monitoring
            }
        }, keyMonitorCts.Token);

        try
        {
            // Wait for the operation to complete
            await operationTask;
            
            // Operation completed, cancel key monitor
            keyMonitorCts.Cancel();
            
            Console.WriteLine("✓ Operation completed successfully\n");
        }
        catch (OperationCanceledException)
        {
            // Cancel key monitor
            keyMonitorCts.Cancel();
            
            Console.WriteLine("✓ OperationCanceledException was caught and handled correctly");
            Console.WriteLine("✓ The operation was cancelled by user request\n");
        }
        finally
        {
            // Ensure key monitor is cancelled
            keyMonitorCts.Cancel();
            
            // Wait briefly for key monitor to finish
            try
            {
                await Task.WhenAny(keyMonitorTask, Task.Delay(200));
            }
            catch { }
            
            // Clear any remaining key presses from buffer
            try
            {
                while (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                }
            }
            catch { }
        }
    }

    /// <summary>
    /// Demonstrates parallel query execution with Task.WhenAll
    /// </summary>
    public async Task DemonstrateParallelExecutionAsync()
    {
        Console.WriteLine("\n" + new string('═', 70));
        Console.WriteLine("DEMONSTRATION 3: Parallel Query Execution with Task.WhenAll");
        Console.WriteLine(new string('═', 70));
        Console.WriteLine("This will execute 5 queries in parallel, each taking 2 seconds.");
        Console.WriteLine("Total time should be ~2 seconds (parallel) instead of ~10 seconds (sequential).\n");

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        var stopwatch = Stopwatch.StartNew();
        var result = await _service.ExecuteParallelQueriesAsync(
            numberOfQueries: 5,
            sleepSecondsPerQuery: 2,
            cts.Token);
        stopwatch.Stop();

        Console.WriteLine("\n" + new string('─', 70));
        Console.WriteLine("PARALLEL EXECUTION RESULTS:");
        Console.WriteLine(new string('─', 70));
        Console.WriteLine($"Total Queries: {result.TotalQueries}");
        Console.WriteLine($"Successful: {result.SuccessfulQueries}");
        Console.WriteLine($"Failed: {result.FailedQueries}");
        Console.WriteLine($"Total Execution Time: {result.TotalExecutionTime} ms");
        
        if (result.Errors.Any())
        {
            Console.WriteLine("\nErrors:");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"  - {error}");
            }
        }

        // Calculate speedup
        var sequentialTime = result.TotalQueries * 2000; // 2 seconds per query
        var speedup = (double)sequentialTime / result.TotalExecutionTime;
        Console.WriteLine($"\nSpeedup: {speedup:F2}x faster than sequential execution");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstrates handling of failures in parallel execution
    /// </summary>
    public async Task DemonstrateFailureHandlingAsync()
    {
        Console.WriteLine("\n" + new string('═', 70));
        Console.WriteLine("DEMONSTRATION 4: Failure Handling in Parallel Execution");
        Console.WriteLine(new string('═', 70));
        Console.WriteLine("This will execute 6 queries in parallel.");
        Console.WriteLine("Every 3rd query will fail (simulated).");
        Console.WriteLine("The system should handle failures gracefully.\n");

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        var result = await _service.ExecuteParallelQueriesAsync(
            numberOfQueries: 6,
            sleepSecondsPerQuery: 1,
            cts.Token);

        Console.WriteLine("\n" + new string('─', 70));
        Console.WriteLine("FAILURE HANDLING RESULTS:");
        Console.WriteLine(new string('─', 70));
        Console.WriteLine($"Total Queries: {result.TotalQueries}");
        Console.WriteLine($"Successful: {result.SuccessfulQueries}");
        Console.WriteLine($"Failed: {result.FailedQueries}");
        Console.WriteLine($"Total Execution Time: {result.TotalExecutionTime} ms");
        
        if (result.Errors.Any())
        {
            Console.WriteLine("\nErrors (expected for every 3rd query):");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"  - {error}");
            }
        }

        Console.WriteLine("\n✓ System continued execution despite individual query failures");
        Console.WriteLine("✓ Other queries completed successfully\n");
    }

    /// <summary>
    /// Demonstrates cancellation in ExecuteReaderAsync and ReadAsync
    /// </summary>
    public async Task DemonstrateReaderCancellationAsync()
    {
        Console.WriteLine("\n" + new string('═', 70));
        Console.WriteLine("DEMONSTRATION 5: Cancellation in ExecuteReaderAsync and ReadAsync");
        Console.WriteLine(new string('═', 70));
        Console.WriteLine("This will start reading data with a 2-second timeout.");
        Console.WriteLine("The operation should be cancelled during data reading.\n");

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

        try
        {
            var stopwatch = Stopwatch.StartNew();
            var results = await _service.ExecuteLongRunningQueryAsync(5, cts.Token);
            stopwatch.Stop();
            
            Console.WriteLine($"✓ Query completed: {results.Count} rows read");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("✓ OperationCanceledException was caught during data reading");
            Console.WriteLine("✓ Cancellation token was properly propagated to ReadAsync\n");
        }
    }

    /// <summary>
    /// Runs all demonstrations
    /// </summary>
    public async Task RunAllDemonstrationsAsync()
    {
        Console.WriteLine("\n" + new string('█', 70));
        Console.WriteLine("ASYNC OPERATIONS WITH CANCELLATION TOKEN DEMONSTRATIONS");
        Console.WriteLine(new string('█', 70));

        await DemonstrateTimeoutCancellationAsync();
        await Task.Delay(1000); 

        await DemonstrateManualCancellationAsync();
        await Task.Delay(1000);

        await DemonstrateParallelExecutionAsync();
        await Task.Delay(1000);

        await DemonstrateFailureHandlingAsync();
        await Task.Delay(1000);

        await DemonstrateReaderCancellationAsync();
    }
}

