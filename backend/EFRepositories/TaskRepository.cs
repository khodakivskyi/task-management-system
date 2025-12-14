using backend.EFModels;
using Microsoft.EntityFrameworkCore;

namespace backend.EFRepositories;

/// <summary>
/// Repository implementation for Task entity using Entity Framework Core
/// Demonstrates repository pattern on top of scaffolded entities
/// </summary>
public class TaskRepository : ITaskRepository
{
    private readonly TaskManagementDbContext _context;

    public TaskRepository(TaskManagementDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<TaskModel?> GetByIdAsync(int id)
    {
        return await _context.Tasks
            .Include(t => t.Owner)
            .Include(t => t.Status)
            .Include(t => t.Category)
            .Include(t => t.Project)
            .Include(t => t.TaskAssignees)
                .ThenInclude(ta => ta.User)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<TaskModel>> GetAllAsync()
    {
        return await _context.Tasks
            .Include(t => t.Owner)
            .Include(t => t.Status)
            .Include(t => t.Category)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskModel>> GetByProjectIdAsync(int projectId)
    {
        return await _context.GetTasksByProjectId(projectId).ToListAsync();
    }

    public async Task<IEnumerable<TaskModel>> GetByOwnerIdAsync(int ownerId)
    {
        return await _context.Tasks
            .Where(t => t.OwnerId == ownerId)
            .Include(t => t.Status)
            .Include(t => t.Category)
            .Include(t => t.Project)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<TaskModel> CreateAsync(TaskModel task)
    {
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<TaskModel> UpdateAsync(TaskModel task)
    {
        task.UpdatedAt = DateTime.UtcNow;
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
        {
            return false;
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> CountAsync()
    {
        return await _context.Tasks.CountAsync();
    }
}

