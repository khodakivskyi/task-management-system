using backend.EFModels;
using Microsoft.EntityFrameworkCore;

namespace backend.EFRepositories;

/// <summary>
/// Demonstrates two approaches to Update operations in Entity Framework Core:
/// 1. Tracked entity (automatic change detection)
/// 2. Detached entity (manual tracking with Update())
/// </summary>
public class RepositoryUpdateDemo
{
    private readonly TaskManagementDbContext _context;

    public RepositoryUpdateDemo(TaskManagementDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Method 1: Tracked entity (automatic change detection)
    /// Entity is already being tracked, EF Core automatically detects changes
    /// </summary>
    public async Task<TaskModel> UpdateTrackedAsync(TaskModel task, CancellationToken cancellationToken = default)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));

        // Get entity from context - it will be tracked
        var existing = await _context.Set<TaskModel>()
            .FindAsync(new object[] { task.Id }, cancellationToken);

        if (existing == null)
            throw new InvalidOperationException($"Task with Id {task.Id} not found");

        // EF Core automatically tracks changes to existing entity
        existing.Title = task.Title;
        existing.Description = task.Description;
        existing.Priority = task.Priority;
        existing.Deadline = task.Deadline;
        existing.StatusId = task.StatusId;
        existing.CategoryId = task.CategoryId;
        existing.ProjectId = task.ProjectId;
        existing.EstimatedHours = task.EstimatedHours;
        existing.ActualHours = task.ActualHours;
        existing.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        // EF Core detects changes automatically for tracked entities
        await _context.SaveChangesAsync(cancellationToken);
        return existing;
    }

    /// <summary>
    /// Method 2: Detached entity (manual tracking)
    /// Entity is not tracked, must use Update() to attach and mark as modified
    /// </summary>
    public async Task<TaskModel> UpdateDetachedAsync(TaskModel task, CancellationToken cancellationToken = default)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));

        // Update() attaches the entity to the context and marks all properties as modified
        task.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _context.Set<TaskModel>().Update(task); // Attach and mark as modified
        await _context.SaveChangesAsync(cancellationToken);
        return task;
    }

    /// <summary>
    /// Demonstrates both update approaches
    /// </summary>
    public async Task DemonstrateBothApproachesAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== Update Methods Comparison ===\n");

        // Get a task to update
        var task = await _context.Set<TaskModel>()
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (task == null)
        {
            Console.WriteLine("No tasks found. Cannot demonstrate update methods.\n");
            return;
        }

        Console.WriteLine($"Original Task ID: {task.Id}, Title: {task.Title}\n");

        // Method 1: Tracked Entity
        Console.WriteLine("Method 1: Tracked Entity (Automatic Change Detection)");
        Console.WriteLine("  - Entity is loaded and tracked by DbContext");
        Console.WriteLine("  - Changes to properties are automatically detected");
        Console.WriteLine("  - No need to call Update() method");
        Console.WriteLine("  - More efficient for entities already in context\n");

        var trackedTask = await _context.Set<TaskModel>()
            .FindAsync(new object[] { task.Id }, cancellationToken);

        if (trackedTask != null)
        {
            trackedTask.Title = "Updated via Tracked Entity";
            trackedTask.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            await _context.SaveChangesAsync(cancellationToken);
            Console.WriteLine($"  ✓ Updated Task Title: {trackedTask.Title}\n");
        }

        // Method 2: Detached Entity
        Console.WriteLine("Method 2: Detached Entity (Manual Tracking)");
        Console.WriteLine("  - Entity is not tracked by DbContext");
        Console.WriteLine("  - Must use Update() method to attach and mark as modified");
        Console.WriteLine("  - Useful when entity comes from outside (API, DTO, etc.)");
        Console.WriteLine("  - All properties are marked as modified\n");

        var detachedTask = new TaskModel
        {
            Id = task.Id,
            OwnerId = task.OwnerId,
            StatusId = task.StatusId,
            Title = "Updated via Detached Entity",
            Description = task.Description,
            Priority = task.Priority,
            Deadline = task.Deadline,
            CreatedAt = task.CreatedAt,
            UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
            EstimatedHours = task.EstimatedHours,
            ActualHours = task.ActualHours,
            CategoryId = task.CategoryId,
            ProjectId = task.ProjectId
        };

        _context.Set<TaskModel>().Update(detachedTask);
        await _context.SaveChangesAsync(cancellationToken);
        Console.WriteLine($"  ✓ Updated Task Title: {detachedTask.Title}\n");

        Console.WriteLine("=== Update Methods Comparison Completed ===\n");
    }
}

