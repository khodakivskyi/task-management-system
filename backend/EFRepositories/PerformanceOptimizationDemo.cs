using backend.EFModels;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace backend.EFRepositories;

/// <summary>
/// Demonstrates EF Core performance optimization techniques:
/// - Compiled Queries
/// - AsNoTracking comparison
/// - Query Splitting (single vs split)
/// - Benchmarking with BenchmarkDotNet
/// </summary>
public class PerformanceOptimizationDemo
{
    private readonly TaskManagementDbContext _context;

    // Compiled queries for hot path operations
    private static readonly Func<TaskManagementDbContext, int, Task<TaskModel?>> _getTaskByIdCompiled =
        EF.CompileAsyncQuery((TaskManagementDbContext context, int id) =>
            context.Set<TaskModel>()
                .Include(t => t.Owner)
                .Include(t => t.Status)
                .Include(t => t.Category)
                .FirstOrDefault(t => t.Id == id));

    private static readonly Func<TaskManagementDbContext, int, int, Task<List<TaskModel>>> _getTasksByOwnerCompiled =
        EF.CompileAsyncQuery((TaskManagementDbContext context, int ownerId, int limit) =>
            context.Set<TaskModel>()
                .Where(t => t.OwnerId == ownerId)
                .OrderByDescending(t => t.CreatedAt)
                .Take(limit)
                .ToList());

    public PerformanceOptimizationDemo(TaskManagementDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Demonstrates compiled queries vs regular LINQ queries
    /// Shows performance benefits for frequently executed queries
    /// </summary>
    public async Task DemonstrateCompiledQueriesAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== Compiled Queries Performance Comparison ===\n");

        // Get a valid task ID for testing
        var testTask = await _context.Set<TaskModel>()
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (testTask == null)
        {
            Console.WriteLine("No tasks found. Cannot demonstrate compiled queries.\n");
            return;
        }

        var taskId = testTask.Id;
        var iterations = 100;

        Console.WriteLine($"Testing with Task ID: {taskId}");
        Console.WriteLine($"Iterations: {iterations}\n");

        // Warm up
        await _context.Set<TaskModel>().AsNoTracking().FirstOrDefaultAsync(cancellationToken);

        // Test 1: Regular LINQ Query
        Console.WriteLine("1. Regular LINQ Query (with Include):");
        var sw1 = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            await _context.Set<TaskModel>()
                .Include(t => t.Owner)
                .Include(t => t.Status)
                .Include(t => t.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);
        }
        sw1.Stop();
        var regularTime = sw1.ElapsedMilliseconds;
        Console.WriteLine($"   Time: {regularTime} ms");
        Console.WriteLine($"   Average: {regularTime / (double)iterations:F2} ms per query\n");

        // Test 2: Compiled Query
        Console.WriteLine("2. Compiled Query (EF.CompileAsyncQuery):");
        var sw2 = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            await _getTaskByIdCompiled(_context, taskId);
        }
        sw2.Stop();
        var compiledTime = sw2.ElapsedMilliseconds;
        Console.WriteLine($"   Time: {compiledTime} ms");
        Console.WriteLine($"   Average: {compiledTime / (double)iterations:F2} ms per query\n");

        // Comparison
        var improvement = ((regularTime - compiledTime) / (double)regularTime) * 100;
        Console.WriteLine("=== Comparison ===");
        Console.WriteLine($"Regular LINQ: {regularTime} ms");
        Console.WriteLine($"Compiled Query: {compiledTime} ms");
        Console.WriteLine($"Improvement: {improvement:F1}% faster");
        Console.WriteLine($"Speedup: {regularTime / (double)compiledTime:F2}x\n");

        Console.WriteLine("=== Compiled Queries Demo Completed ===\n");
    }

    /// <summary>
    /// Demonstrates AsNoTracking vs Tracking performance comparison
    /// Measures execution time and memory usage
    /// </summary>
    public async Task DemonstrateAsNoTrackingComparisonAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== AsNoTracking vs Tracking Performance Comparison ===\n");

        var iterations = 50;

        Console.WriteLine($"Iterations: {iterations}\n");

        // Test 1: With Tracking (default)
        Console.WriteLine("1. Query WITH Tracking (default):");
        Console.WriteLine("   - EF Core tracks all entities");
        Console.WriteLine("   - Higher memory usage");
        Console.WriteLine("   - Slower query execution\n");

        var sw1 = Stopwatch.StartNew();
        var memoryBefore1 = GC.GetTotalMemory(false);
        
        for (int i = 0; i < iterations; i++)
        {
            var tasks = await _context.Set<TaskModel>()
                .Include(t => t.Owner)
                .Include(t => t.Status)
                .Take(100)
                .ToListAsync(cancellationToken);
        }

        sw1.Stop();
        var memoryAfter1 = GC.GetTotalMemory(false);
        var memoryUsed1 = memoryAfter1 - memoryBefore1;
        var trackingTime = sw1.ElapsedMilliseconds;

        Console.WriteLine($"   Execution Time: {trackingTime} ms");
        Console.WriteLine($"   Average: {trackingTime / (double)iterations:F2} ms per query");
        Console.WriteLine($"   Memory Used: {memoryUsed1 / 1024.0:F2} KB\n");

        // Force garbage collection
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        await Task.Delay(100, cancellationToken);

        // Test 2: With AsNoTracking
        Console.WriteLine("2. Query WITH AsNoTracking():");
        Console.WriteLine("   - EF Core does NOT track entities");
        Console.WriteLine("   - Lower memory usage");
        Console.WriteLine("   - Faster query execution\n");

        var sw2 = Stopwatch.StartNew();
        var memoryBefore2 = GC.GetTotalMemory(false);

        for (int i = 0; i < iterations; i++)
        {
            var tasks = await _context.Set<TaskModel>()
                .AsNoTracking()
                .Include(t => t.Owner)
                .Include(t => t.Status)
                .Take(100)
                .ToListAsync(cancellationToken);
        }

        sw2.Stop();
        var memoryAfter2 = GC.GetTotalMemory(false);
        var memoryUsed2 = memoryAfter2 - memoryBefore2;
        var noTrackingTime = sw2.ElapsedMilliseconds;

        Console.WriteLine($"   Execution Time: {noTrackingTime} ms");
        Console.WriteLine($"   Average: {noTrackingTime / (double)iterations:F2} ms per query");
        Console.WriteLine($"   Memory Used: {memoryUsed2 / 1024.0:F2} KB\n");

        // Comparison
        var timeImprovement = ((trackingTime - noTrackingTime) / (double)trackingTime) * 100;
        var memoryImprovement = ((memoryUsed1 - memoryUsed2) / (double)memoryUsed1) * 100;

        Console.WriteLine("=== Comparison ===");
        Console.WriteLine($"Tracking Time: {trackingTime} ms");
        Console.WriteLine($"AsNoTracking Time: {noTrackingTime} ms");
        Console.WriteLine($"Time Improvement: {timeImprovement:F1}% faster");
        Console.WriteLine($"Time Speedup: {trackingTime / (double)noTrackingTime:F2}x\n");

        Console.WriteLine($"Tracking Memory: {memoryUsed1 / 1024.0:F2} KB");
        Console.WriteLine($"AsNoTracking Memory: {memoryUsed2 / 1024.0:F2} KB");
        Console.WriteLine($"Memory Improvement: {memoryImprovement:F1}% less memory\n");

        Console.WriteLine("=== AsNoTracking Comparison Demo Completed ===\n");
    }

    /// <summary>
    /// Demonstrates query splitting: single query vs split query
    /// Analyzes generated SQL queries
    /// </summary>
    public async Task DemonstrateQuerySplittingAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== Query Splitting: Single Query vs Split Query ===\n");

        Console.WriteLine("Scenario: Loading tasks with multiple collections (TaskAssignees, Comments)\n");

        // Test 1: Single Query (default behavior)
        Console.WriteLine("1. Single Query (default - AsSingleQuery()):");
        Console.WriteLine("   - All data loaded in one SQL query with JOINs");
        Console.WriteLine("   - Can cause Cartesian explosion with multiple collections");
        Console.WriteLine("   - More data transferred\n");

        var sw1 = Stopwatch.StartNew();
        var singleQueryTasks = await _context.Set<TaskModel>()
            .AsNoTracking()
            .Include(t => t.Owner)
            .Include(t => t.Status)
            .Include(t => t.TaskAssignees)
                .ThenInclude(ta => ta.User)
            .Include(t => t.Comments)
                .ThenInclude(c => c.User)
            .Take(10)
            .AsSingleQuery()
            .ToListAsync(cancellationToken);
        sw1.Stop();

        Console.WriteLine($"   Loaded {singleQueryTasks.Count} tasks");
        Console.WriteLine($"   Execution Time: {sw1.ElapsedMilliseconds} ms");
        Console.WriteLine($"   Total TaskAssignees: {singleQueryTasks.Sum(t => t.TaskAssignees?.Count ?? 0)}");
        Console.WriteLine($"   Total Comments: {singleQueryTasks.Sum(t => t.Comments?.Count ?? 0)}\n");

        // Test 2: Split Query
        Console.WriteLine("2. Split Query (AsSplitQuery()):");
        Console.WriteLine("   - Multiple SQL queries executed");
        Console.WriteLine("   - Avoids Cartesian explosion");
        Console.WriteLine("   - More efficient for multiple collections");
        Console.WriteLine("   - Less data transferred per query\n");

        var sw2 = Stopwatch.StartNew();
        var splitQueryTasks = await _context.Set<TaskModel>()
            .AsNoTracking()
            .Include(t => t.Owner)
            .Include(t => t.Status)
            .Include(t => t.TaskAssignees)
                .ThenInclude(ta => ta.User)
            .Include(t => t.Comments)
                .ThenInclude(c => c.User)
            .Take(10)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
        sw2.Stop();

        Console.WriteLine($"   Loaded {splitQueryTasks.Count} tasks");
        Console.WriteLine($"   Execution Time: {sw2.ElapsedMilliseconds} ms");
        Console.WriteLine($"   Total TaskAssignees: {splitQueryTasks.Sum(t => t.TaskAssignees?.Count ?? 0)}");
        Console.WriteLine($"   Total Comments: {splitQueryTasks.Sum(t => t.Comments?.Count ?? 0)}\n");

        // Comparison
        Console.WriteLine("=== Comparison ===");
        Console.WriteLine($"Single Query Time: {sw1.ElapsedMilliseconds} ms");
        Console.WriteLine($"Split Query Time: {sw2.ElapsedMilliseconds} ms");

        if (sw1.ElapsedMilliseconds < sw2.ElapsedMilliseconds)
        {
            var improvement = ((sw2.ElapsedMilliseconds - sw1.ElapsedMilliseconds) / (double)sw2.ElapsedMilliseconds) * 100;
            Console.WriteLine($"Single Query is {improvement:F1}% faster (better for small datasets)\n");
        }
        else
        {
            var improvement = ((sw1.ElapsedMilliseconds - sw2.ElapsedMilliseconds) / (double)sw1.ElapsedMilliseconds) * 100;
            Console.WriteLine($"Split Query is {improvement:F1}% faster (better for multiple collections)\n");
        }

        Console.WriteLine("=== When to Use Each Approach ===");
        Console.WriteLine("Single Query (AsSingleQuery):");
        Console.WriteLine("  - Few related entities");
        Console.WriteLine("  - Simple relationships");
        Console.WriteLine("  - Small datasets");
        Console.WriteLine("  - Fewer round trips to database\n");

        Console.WriteLine("Split Query (AsSplitQuery):");
        Console.WriteLine("  - Multiple collections (one-to-many)");
        Console.WriteLine("  - Risk of Cartesian explosion");
        Console.WriteLine("  - Large datasets with many related entities");
        Console.WriteLine("  - Better performance for complex relationships\n");

        Console.WriteLine("=== Query Splitting Demo Completed ===\n");
    }

    /// <summary>
    /// Runs BenchmarkDotNet benchmarks for accurate performance measurements
    /// </summary>
    public void RunBenchmarks()
    {
        Console.WriteLine("=== Running BenchmarkDotNet Benchmarks ===\n");
        Console.WriteLine("This will run comprehensive benchmarks for:");
        Console.WriteLine("  - Compiled Queries vs Regular LINQ");
        Console.WriteLine("  - AsNoTracking vs Tracking");
        Console.WriteLine("  - Single Query vs Split Query\n");

        var summary = BenchmarkRunner.Run<EFCoreBenchmarks>();

        Console.WriteLine("\n=== Benchmark Results Summary ===");
        Console.WriteLine("Check the output above for detailed benchmark results.");
        Console.WriteLine("Results are displayed in a table format showing:");
        Console.WriteLine("  - Mean execution time");
        Console.WriteLine("  - Error margin");
        Console.WriteLine("  - Standard deviation");
        Console.WriteLine("  - Memory allocation\n");
    }

    /// <summary>
    /// Runs all performance optimization demonstrations
    /// </summary>
    public async Task RunAllDemonstrationsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await DemonstrateCompiledQueriesAsync(cancellationToken);
            await DemonstrateAsNoTrackingComparisonAsync(cancellationToken);
            await DemonstrateQuerySplittingAsync(cancellationToken);

            Console.WriteLine("=== All Performance Optimization Demonstrations Completed ===");
            Console.WriteLine("\nNote: For more accurate measurements, run BenchmarkDotNet benchmarks separately.");
            Console.WriteLine("Call RunBenchmarks() method to execute comprehensive benchmarks.\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during demonstration: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}

/// <summary>
/// BenchmarkDotNet benchmark class for EF Core performance optimization techniques
/// </summary>
[MemoryDiagnoser]
public class EFCoreBenchmarks
{
    private TaskManagementDbContext? _context;
    private int _testTaskId;
    private int _testOwnerId;

    // Compiled queries
    private static readonly Func<TaskManagementDbContext, int, Task<TaskModel?>> _getTaskByIdCompiled =
        EF.CompileAsyncQuery((TaskManagementDbContext context, int id) =>
            context.Set<TaskModel>()
                .Include(t => t.Owner)
                .Include(t => t.Status)
                .FirstOrDefault(t => t.Id == id));

    [GlobalSetup]
    public async Task Setup()
    {
        // Load .env file
        DotNetEnv.Env.Load();

        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
            ?? throw new InvalidOperationException("DB_CONNECTION_STRING is not set");

        var options = new DbContextOptionsBuilder<TaskManagementDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        _context = new TaskManagementDbContext(options);

        // Get test data IDs
        var testTask = await _context.Set<TaskModel>()
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (testTask != null)
        {
            _testTaskId = testTask.Id;
            _testOwnerId = testTask.OwnerId;
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _context?.Dispose();
    }

    [Benchmark(Baseline = true)]
    public async Task RegularLinqQuery()
    {
        if (_context == null) return;

        await _context.Set<TaskModel>()
            .Include(t => t.Owner)
            .Include(t => t.Status)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == _testTaskId);
    }

    [Benchmark]
    public async Task CompiledQuery()
    {
        if (_context == null) return;

        await _getTaskByIdCompiled(_context, _testTaskId);
    }

    [Benchmark]
    public async Task QueryWithTracking()
    {
        if (_context == null) return;

        await _context.Set<TaskModel>()
            .Include(t => t.Owner)
            .Include(t => t.Status)
            .Take(50)
            .ToListAsync();
    }

    [Benchmark]
    public async Task QueryWithAsNoTracking()
    {
        if (_context == null) return;

        await _context.Set<TaskModel>()
            .AsNoTracking()
            .Include(t => t.Owner)
            .Include(t => t.Status)
            .Take(50)
            .ToListAsync();
    }

    [Benchmark]
    public async Task SingleQueryWithMultipleIncludes()
    {
        if (_context == null) return;

        await _context.Set<TaskModel>()
            .AsNoTracking()
            .Include(t => t.Owner)
            .Include(t => t.Status)
            .Include(t => t.TaskAssignees)
            .Include(t => t.Comments)
            .Take(10)
            .AsSingleQuery()
            .ToListAsync();
    }

    [Benchmark]
    public async Task SplitQueryWithMultipleIncludes()
    {
        if (_context == null) return;

        await _context.Set<TaskModel>()
            .AsNoTracking()
            .Include(t => t.Owner)
            .Include(t => t.Status)
            .Include(t => t.TaskAssignees)
            .Include(t => t.Comments)
            .Take(10)
            .AsSplitQuery()
            .ToListAsync();
    }
}

