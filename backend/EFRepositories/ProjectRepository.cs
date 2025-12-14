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

    public async Task<Project?> GetByIdAsync(int id)
    {
        return await _context.Projects
            .Include(p => p.Owner)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Project?> GetWithDetailsAsync(int projectId)
    {
        return await _context.GetProjectWithDetailsAsync(projectId);
    }

    public async Task<IEnumerable<Project>> GetAllAsync()
    {
        return await _context.Projects
            .Include(p => p.Owner)
            .OrderByDescending(p => p.StartDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetByOwnerIdAsync(int ownerId)
    {
        return await _context.Projects
            .Where(p => p.OwnerId == ownerId)
            .Include(p => p.Owner)
            .OrderByDescending(p => p.StartDate)
            .ToListAsync();
    }

    public async Task<Project> CreateAsync(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<Project> UpdateAsync(Project project)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
        {
            return false;
        }

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return true;
    }
}

