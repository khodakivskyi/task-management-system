using backend.EFModels;
using Microsoft.EntityFrameworkCore;

namespace backend.EFRepositories;

/// <summary>
/// Repository implementation for Project entity using Entity Framework Core
/// </summary>
public class ProjectRepository : IProjectRepository
{
    private readonly TaskManagementDbContext _context;

    public ProjectRepository(TaskManagementDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets a project by ID using FindAsync() for tracked entity
    /// </summary>
    public async Task<Project?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Project>()
            .FindAsync(new object[] { id }, cancellationToken);
    }

    /// <summary>
    /// Gets all projects with optional change tracking
    /// Uses AsNoTracking() when trackChanges = false for better performance in read-only scenarios
    /// </summary>
    public async Task<IEnumerable<Project>> GetAllAsync(bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<Project>().AsQueryable();

        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return await query
            .OrderByDescending(p => p.StartDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Creates a new project using context.Set<T>().Add() and SaveChangesAsync()
    /// </summary>
    public async Task<Project> CreateAsync(Project project, CancellationToken cancellationToken = default)
    {
        if (project == null)
            throw new ArgumentNullException(nameof(project));

        await _context.Set<Project>().AddAsync(project, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return project;
    }

    /// <summary>
    /// Updates an existing project using change tracking
    /// Supports both tracked and detached entities:
    /// - Tracked: Entity is already being tracked, changes are detected automatically
    /// - Detached: Entity is not tracked, Update() attaches and marks as modified
    /// </summary>
    public async Task<Project> UpdateAsync(Project project, CancellationToken cancellationToken = default)
    {
        if (project == null)
            throw new ArgumentNullException(nameof(project));

        // Check if entity is already tracked
        var existing = await _context.Set<Project>()
            .FindAsync(new object[] { project.Id }, cancellationToken);

        if (existing == null)
            throw new InvalidOperationException($"Project with Id {project.Id} not found");

        // Method 1: Tracked entity (automatic change detection)
        // EF Core automatically tracks changes to existing entity
        existing.Name = project.Name;
        existing.Description = project.Description;
        existing.StartDate = project.StartDate;
        existing.EndDate = project.EndDate;
        existing.OwnerId = project.OwnerId;

        // EF Core detects changes automatically for tracked entities
        await _context.SaveChangesAsync(cancellationToken);
        return existing;
    }

    /// <summary>
    /// Deletes a project by ID using Remove() and SaveChangesAsync()
    /// </summary>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var project = await _context.Set<Project>()
            .FindAsync(new object[] { id }, cancellationToken);

        if (project == null)
        {
            return false;
        }

        _context.Set<Project>().Remove(project);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

