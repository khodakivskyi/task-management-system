using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using backend.EFModels;

namespace backend;

/// <summary>
/// Partial class for TaskManagementDbContext to add custom logic
/// This allows us to extend the scaffolded DbContext without modifying the generated code
/// </summary>
public partial class TaskManagementDbContext
{
    private static bool _envLoaded = false;
    private static readonly object _envLock = new object();

    /// <summary>
    /// Override OnConfiguring to use connection string from .env file
    /// This removes the hardcoded connection string from the scaffolded code
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Only configure if not already configured (allows dependency injection)
        if (!optionsBuilder.IsConfigured)
        {
            // Load .env file if not already loaded (thread-safe)
            if (!_envLoaded)
            {
                lock (_envLock)
                {
                    if (!_envLoaded)
                    {
                        Env.Load();
                        _envLoaded = true;
                    }
                }
            }

            // Get connection string from environment variables (loaded from .env file)
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? throw new InvalidOperationException(
                    "Connection string 'DB_CONNECTION_STRING' not found. " +
                    "Please configure it in .env file or environment variables.");

            optionsBuilder.UseNpgsql(connectionString);
        }
    }

    /// <summary>
    /// Custom method to get tasks by project ID with related entities loaded
    /// Demonstrates custom query logic in partial class
    /// </summary>
    public IQueryable<TaskModel> GetTasksByProjectId(int projectId)
    {
        return Tasks
            .Where(t => t.ProjectId == projectId)
            .Include(t => t.Owner)
            .Include(t => t.Status)
            .Include(t => t.Category)
            .Include(t => t.TaskAssignees)
                .ThenInclude(ta => ta.User);
    }

    /// <summary>
    /// Custom method to get tasks by owner ID with pagination
    /// </summary>
    public IQueryable<TaskModel> GetTasksByOwnerId(int ownerId, int skip = 0, int take = 10)
    {
        return Tasks
            .Where(t => t.OwnerId == ownerId)
            .OrderByDescending(t => t.CreatedAt)
            .Skip(skip)
            .Take(take)
            .Include(t => t.Status)
            .Include(t => t.Category)
            .Include(t => t.Project);
    }

    /// <summary>
    /// Custom method to get user with all related projects and tasks
    /// </summary>
    public async Task<User?> GetUserWithDetailsAsync(int userId)
    {
        return await Users
            .Include(u => u.Projects)
            .Include(u => u.Tasks)
            .Include(u => u.ProjectMembers)
                .ThenInclude(pm => pm.Project)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    /// <summary>
    /// Custom method to check if user exists by login
    /// </summary>
    public async Task<bool> UserExistsByLoginAsync(string login)
    {
        return await Users.AnyAsync(u => u.Login == login);
    }

    /// <summary>
    /// Custom method to get project with all members and tasks
    /// </summary>
    public async Task<Project?> GetProjectWithDetailsAsync(int projectId)
    {
        return await Projects
            .Include(p => p.Owner)
            .Include(p => p.ProjectMembers)
                .ThenInclude(pm => pm.User)
            .Include(p => p.ProjectMembers)
                .ThenInclude(pm => pm.Role)
            .Include(p => p.Tasks)
                .ThenInclude(t => t.Status)
            .FirstOrDefaultAsync(p => p.Id == projectId);
    }
}

