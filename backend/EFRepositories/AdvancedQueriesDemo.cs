using backend.EFModels;
using backend.EFRepositories.DTOs;
using Microsoft.EntityFrameworkCore;

namespace backend.EFRepositories;

/// <summary>
/// Demonstrates advanced EF Core query techniques:
/// - Eager loading with Include/ThenInclude
/// - N+1 problem demonstration
/// - Projection to DTO
/// - Pagination
/// - Grouping with aggregation
/// - Complex filtering
/// </summary>
public class AdvancedQueriesDemo
{
    private readonly TaskManagementDbContext _context;
    private readonly ITaskRepository _taskRepository;

    public AdvancedQueriesDemo(TaskManagementDbContext context, ITaskRepository taskRepository)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
    }

    /// <summary>
    /// Demonstrates N+1 problem vs eager loading
    /// </summary>
    public async Task DemonstrateNPlusOneProblemAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== N+1 Problem Demonstration ===\n");

        // BAD: N+1 queries (1 query for tasks + N queries for owners)
        Console.WriteLine("1. BAD APPROACH - N+1 Problem:");
        Console.WriteLine("   Loading tasks without Include() causes separate query for each owner\n");

        var tasksWithoutInclude = await _context.Set<TaskModel>()
            .AsNoTracking()
            .Take(5)
            .ToListAsync(cancellationToken);

        Console.WriteLine($"   Initial query loaded {tasksWithoutInclude.Count} tasks");
        Console.WriteLine("   Now accessing Owner property for each task...\n");

        int queryCount = 1; // Initial query
        foreach (var task in tasksWithoutInclude)
        {
            // Each iteration triggers a separate database query!
            var ownerName = task.Owner?.Name ?? "Unknown";
            queryCount++;
            Console.WriteLine($"   Task '{task.Title}' - Owner: {ownerName} (Query #{queryCount})");
        }

        Console.WriteLine($"\n   Total queries executed: {queryCount} (1 for tasks + {tasksWithoutInclude.Count} for owners)\n");

        // GOOD: Single query with eager loading
        Console.WriteLine("2. GOOD APPROACH - Eager Loading with Include():");
        Console.WriteLine("   Loading tasks with Include() loads all related data in one query\n");

        var tasksWithInclude = await _context.Set<TaskModel>()
            .AsNoTracking()
            .Include(t => t.Owner)
            .Include(t => t.Status)
            .Include(t => t.Category)
            .Take(5)
            .ToListAsync(cancellationToken);

        Console.WriteLine($"   Single query loaded {tasksWithInclude.Count} tasks with all relations");
        Console.WriteLine("   Accessing Owner property - no additional queries needed:\n");

        foreach (var task in tasksWithInclude)
        {
            var ownerName = task.Owner?.Name ?? "Unknown";
            var statusName = task.Status?.Name ?? "Unknown";
            Console.WriteLine($"   Task '{task.Title}' - Owner: {ownerName}, Status: {statusName} (No additional query)");
        }

        Console.WriteLine($"\n   Total queries executed: 1 (all data loaded in single query)\n");
        Console.WriteLine("=== N+1 Problem Demo Completed ===\n");
    }

    /// <summary>
    /// Demonstrates eager loading with nested relations (ThenInclude)
    /// </summary>
    public async Task DemonstrateEagerLoadingAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== Eager Loading with Include/ThenInclude ===\n");

        Console.WriteLine("Loading task with all related entities:");
        Console.WriteLine("  - Owner (Include)");
        Console.WriteLine("  - Status (Include)");
        Console.WriteLine("  - Category (Include)");
        Console.WriteLine("  - Project (Include)");
        Console.WriteLine("  - TaskAssignees -> User (Include + ThenInclude)");
        Console.WriteLine("  - Comments -> User (Include + ThenInclude)\n");

        var task = await _taskRepository.GetByIdWithRelationsAsync(1, cancellationToken);

        if (task != null)
        {
            Console.WriteLine($"Task: {task.Title}");
            Console.WriteLine($"  Owner: {task.Owner?.Name} {task.Owner?.Surname}");
            Console.WriteLine($"  Status: {task.Status?.Name}");
            Console.WriteLine($"  Category: {task.Category?.Name ?? "None"}");
            Console.WriteLine($"  Project: {task.Project?.Name ?? "None"}");
            Console.WriteLine($"  Assignees: {task.TaskAssignees?.Count ?? 0}");
            Console.WriteLine($"  Comments: {task.Comments?.Count ?? 0}\n");
        }
        else
        {
            Console.WriteLine("Task not found\n");
        }

        Console.WriteLine("=== Eager Loading Demo Completed ===\n");
    }

    /// <summary>
    /// Demonstrates projection to DTO
    /// </summary>
    public async Task DemonstrateProjectionAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== Projection to DTO ===\n");

        Console.WriteLine("Benefits of projection:");
        Console.WriteLine("  - Only necessary fields are selected");
        Console.WriteLine("  - Automatic AsNoTracking (no change tracking overhead)");
        Console.WriteLine("  - Less data transferred over network");
        Console.WriteLine("  - Better performance for read-only scenarios\n");

        var taskDtos = await _taskRepository.GetTasksAsDtoAsync(cancellationToken);

        Console.WriteLine($"Loaded {taskDtos.Count()} tasks as DTOs:\n");
        foreach (var dto in taskDtos.Take(5))
        {
            Console.WriteLine($"  ID: {dto.Id}, Title: {dto.Title}");
            Console.WriteLine($"    Owner: {dto.OwnerName}, Status: {dto.StatusName}");
            Console.WriteLine($"    Category: {dto.CategoryName ?? "None"}, Project: {dto.ProjectName ?? "None"}\n");
        }

        Console.WriteLine("=== Projection Demo Completed ===\n");
    }

    /// <summary>
    /// Demonstrates pagination
    /// </summary>
    public async Task DemonstratePaginationAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== Pagination ===\n");

        Console.WriteLine("Page 1 (10 items per page, sorted by CreatedAt desc):");
        var page1 = await _taskRepository.GetPagedAsync(1, 10, "createdat", "desc", cancellationToken);
        Console.WriteLine($"  Total items: {page1.TotalCount}");
        Console.WriteLine($"  Page {page1.PageNumber} of {page1.TotalPages}");
        Console.WriteLine($"  Items on this page: {page1.Items.Count()}");
        Console.WriteLine($"  Has previous: {page1.HasPreviousPage}, Has next: {page1.HasNextPage}\n");

        Console.WriteLine("Page 2 (5 items per page, sorted by Title asc):");
        var page2 = await _taskRepository.GetPagedAsync(2, 5, "title", "asc", cancellationToken);
        Console.WriteLine($"  Total items: {page2.TotalCount}");
        Console.WriteLine($"  Page {page2.PageNumber} of {page2.TotalPages}");
        Console.WriteLine($"  Items on this page: {page2.Items.Count()}\n");

        Console.WriteLine("=== Pagination Demo Completed ===\n");
    }

    /// <summary>
    /// Demonstrates grouping with aggregation
    /// </summary>
    public async Task DemonstrateGroupingAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== Grouping with Aggregation ===\n");

        Console.WriteLine("Tasks grouped by status with statistics:\n");

        var groupedTasks = await _taskRepository.GetTasksGroupedByStatusAsync(cancellationToken);

        foreach (var group in groupedTasks)
        {
            Console.WriteLine($"Status: {group.StatusName} (ID: {group.StatusId})");
            Console.WriteLine($"  Task Count: {group.TaskCount}");
            Console.WriteLine($"  Average Priority: {group.AveragePriority ?? 0}");
            Console.WriteLine($"  Total Estimated Hours: {group.TotalEstimatedHours}");
            Console.WriteLine($"  Total Actual Hours: {group.TotalActualHours}\n");
        }

        Console.WriteLine("=== Grouping Demo Completed ===\n");
    }

    /// <summary>
    /// Demonstrates complex filtering
    /// </summary>
    public async Task DemonstrateFilteringAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== Complex Filtering ===\n");

        // Get first user and status for filtering
        var users = await _context.Set<User>().AsNoTracking().ToListAsync(cancellationToken);
        var statuses = await _context.Set<Status>().AsNoTracking().ToListAsync(cancellationToken);

        if (users.Any() && statuses.Any())
        {
            var userId = users.First().Id;
            var statusId = statuses.First().Id;

            Console.WriteLine($"Filtering tasks by:");
            Console.WriteLine($"  OwnerId: {userId}");
            Console.WriteLine($"  StatusId: {statusId}");
            Console.WriteLine($"  Priority: >= 1 and <= 5\n");

            var filteredTasks = await _taskRepository.GetFilteredTasksAsync(
                ownerId: userId,
                statusId: statusId,
                minPriority: 1,
                maxPriority: 5,
                trackChanges: false,
                cancellationToken: cancellationToken);

            Console.WriteLine($"Found {filteredTasks.Count()} tasks matching criteria:\n");
            foreach (var task in filteredTasks.Take(5))
            {
                Console.WriteLine($"  Task: {task.Title}, Priority: {task.Priority}, Deadline: {task.Deadline?.ToString("yyyy-MM-dd") ?? "None"}");
            }
        }
        else
        {
            Console.WriteLine("No users or statuses found for filtering demo\n");
        }

        Console.WriteLine("\n=== Filtering Demo Completed ===\n");
    }

    /// <summary>
    /// Runs all advanced query demonstrations
    /// </summary>
    public async Task RunAllDemonstrationsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await DemonstrateNPlusOneProblemAsync(cancellationToken);
            await DemonstrateEagerLoadingAsync(cancellationToken);
            await DemonstrateProjectionAsync(cancellationToken);
            await DemonstratePaginationAsync(cancellationToken);
            await DemonstrateGroupingAsync(cancellationToken);
            await DemonstrateFilteringAsync(cancellationToken);

            Console.WriteLine("=== All Advanced Query Demonstrations Completed ===");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during demonstration: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}


