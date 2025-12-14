using backend.EFModels;
using Microsoft.EntityFrameworkCore;

namespace backend.EFRepositories;

/// <summary>
/// Repository implementation for Task entity using Entity Framework Core
/// Demonstrates proper change tracking, AsNoTracking, and async operations with CancellationToken
/// </summary>
public class TaskRepository : ITaskRepository
{
    private readonly TaskManagementDbContext _context;

    public TaskRepository(TaskManagementDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets a task by ID using FindAsync() for tracked entity
    /// </summary>
    public async Task<TaskModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<TaskModel>()
            .FindAsync(new object[] { id }, cancellationToken);
    }

    /// <summary>
    /// Gets all tasks with optional change tracking
    /// Uses AsNoTracking() when trackChanges = false for better performance in read-only scenarios
    /// </summary>
    public async Task<IEnumerable<TaskModel>> GetAllAsync(bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<TaskModel>().AsQueryable();

        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Creates a new task using context.Set<T>().Add() and SaveChangesAsync()
    /// </summary>
    public async Task<TaskModel> CreateAsync(TaskModel task, CancellationToken cancellationToken = default)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));

        task.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        task.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        await _context.Set<TaskModel>().AddAsync(task, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return task;
    }

    /// <summary>
    /// Updates an existing task using change tracking
    /// Supports both tracked and detached entities:
    /// - Tracked: Entity is already being tracked, changes are detected automatically
    /// - Detached: Entity is not tracked, Update() attaches and marks as modified
    /// </summary>
    public async Task<TaskModel> UpdateAsync(TaskModel task, CancellationToken cancellationToken = default)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));

        // Check if entity is already tracked
        var existing = await _context.Set<TaskModel>()
            .FindAsync(new object[] { task.Id }, cancellationToken);

        if (existing == null)
            throw new InvalidOperationException($"Task with Id {task.Id} not found");

        // Method 1: Tracked entity (automatic change detection)
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
    /// Deletes a task by ID using Remove() and SaveChangesAsync()
    /// </summary>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var task = await _context.Set<TaskModel>()
            .FindAsync(new object[] { id }, cancellationToken);

        if (task == null)
        {
            return false;
        }

        _context.Set<TaskModel>().Remove(task);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

