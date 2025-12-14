using backend.EFModels;
using backend.EFRepositories.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

    /// <summary>
    /// Gets a task by ID with all related entities using eager loading (Include/ThenInclude)
    /// Demonstrates eager loading to avoid N+1 problem
    /// </summary>
    public async Task<TaskModel?> GetByIdWithRelationsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<TaskModel>()
            .Include(t => t.Owner)
            .Include(t => t.Status)
            .Include(t => t.Category)
            .Include(t => t.Project)
            .Include(t => t.TaskAssignees)
                .ThenInclude(ta => ta.User)
            .Include(t => t.Comments)
                .ThenInclude(c => c.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    /// <summary>
    /// Gets all tasks with all related entities using eager loading
    /// Uses Include() and ThenInclude() to load related data in a single query
    /// </summary>
    public async Task<IEnumerable<TaskModel>> GetAllWithRelationsAsync(bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<TaskModel>()
            .Include(t => t.Owner)
            .Include(t => t.Status)
            .Include(t => t.Category)
            .Include(t => t.Project)
            .Include(t => t.TaskAssignees)
                .ThenInclude(ta => ta.User)
            .AsQueryable();

        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets tasks as DTO using projection (Select)
    /// Benefits: less data transferred, automatic AsNoTracking, better performance
    /// </summary>
    public async Task<IEnumerable<TaskDto>> GetTasksAsDtoAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<TaskModel>()
            .AsNoTracking()
            .Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Priority = t.Priority,
                Deadline = t.Deadline,
                OwnerName = t.Owner.Name + " " + (t.Owner.Surname ?? ""),
                StatusName = t.Status.Name,
                CategoryName = t.Category != null ? t.Category.Name : null,
                ProjectName = t.Project != null ? t.Project.Name : null,
                CreatedAt = t.CreatedAt,
                EstimatedHours = t.EstimatedHours,
                ActualHours = t.ActualHours
            })
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets paged tasks with dynamic sorting
    /// Returns both data and total count for pagination UI
    /// </summary>
    public async Task<PagedResult<TaskModel>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? sortBy = null,
        string? sortDirection = "asc",
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _context.Set<TaskModel>().AsNoTracking().AsQueryable();

        // Dynamic sorting
        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            var isDescending = sortDirection?.ToLower() == "desc";
            query = sortBy.ToLower() switch
            {
                "title" => isDescending ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
                "createdat" => isDescending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt),
                "priority" => isDescending ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),
                "deadline" => isDescending ? query.OrderByDescending(t => t.Deadline) : query.OrderBy(t => t.Deadline),
                _ => query.OrderByDescending(t => t.CreatedAt) // Default sorting
            };
        }
        else
        {
            query = query.OrderByDescending(t => t.CreatedAt);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TaskModel>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Groups tasks by status with aggregation (Count, Average, Sum)
    /// Demonstrates GroupBy with aggregation functions
    /// </summary>
    public async Task<IEnumerable<TaskGroupByStatusDto>> GetTasksGroupedByStatusAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<TaskModel>()
            .AsNoTracking()
            .GroupBy(t => new { t.StatusId, t.Status.Name })
            .Select(g => new TaskGroupByStatusDto
            {
                StatusId = g.Key.StatusId,
                StatusName = g.Key.Name,
                TaskCount = g.Count(),
                AveragePriority = (int?)g.Average(t => (int?)t.Priority),
                TotalEstimatedHours = g.Sum(t => t.EstimatedHours),
                TotalActualHours = g.Sum(t => t.ActualHours)
            })
            .OrderByDescending(g => g.TaskCount)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets filtered tasks with dynamic WHERE conditions
    /// Combines multiple filter conditions using && and ||
    /// </summary>
    public async Task<IEnumerable<TaskModel>> GetFilteredTasksAsync(
        int? ownerId = null,
        int? statusId = null,
        int? projectId = null,
        int? minPriority = null,
        int? maxPriority = null,
        DateTime? deadlineFrom = null,
        DateTime? deadlineTo = null,
        bool trackChanges = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<TaskModel>().AsQueryable();

        // Dynamic filtering - build WHERE conditions based on parameters
        if (ownerId.HasValue)
        {
            query = query.Where(t => t.OwnerId == ownerId.Value);
        }

        if (statusId.HasValue)
        {
            query = query.Where(t => t.StatusId == statusId.Value);
        }

        if (projectId.HasValue)
        {
            query = query.Where(t => t.ProjectId == projectId.Value);
        }

        if (minPriority.HasValue)
        {
            query = query.Where(t => t.Priority.HasValue && t.Priority >= minPriority.Value);
        }

        if (maxPriority.HasValue)
        {
            query = query.Where(t => t.Priority.HasValue && t.Priority <= maxPriority.Value);
        }

        if (deadlineFrom.HasValue)
        {
            query = query.Where(t => t.Deadline.HasValue && t.Deadline >= deadlineFrom.Value);
        }

        if (deadlineTo.HasValue)
        {
            query = query.Where(t => t.Deadline.HasValue && t.Deadline <= deadlineTo.Value);
        }

        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}

