using backend.EFModels;
using Microsoft.EntityFrameworkCore;

namespace backend.EFRepositories;

/// <summary>
/// Demonstration class showing CRUD operations with Entity Framework Core
/// Demonstrates:
/// - Creating new records
/// - Reading by ID
/// - Updating existing records (tracked and detached approaches)
/// - Deleting records
/// - Using AsNoTracking for read-only operations
/// - Proper use of CancellationToken
/// </summary>
public class RepositoryDemo
{
    private readonly TaskManagementDbContext _context;
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProjectRepository _projectRepository;

    public RepositoryDemo(
        TaskManagementDbContext context,
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        IProjectRepository projectRepository)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
    }

    /// <summary>
    /// Demonstrates all CRUD operations for Task entity
    /// </summary>
    public async Task DemonstrateTaskOperationsAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== Task Repository CRUD Operations Demo ===\n");

        // Get existing user and status from database
        var users = await _userRepository.GetAllAsync(trackChanges: false, cancellationToken);
        var owner = users.FirstOrDefault();
        if (owner == null)
        {
            Console.WriteLine("   No users found. Cannot create task.\n");
            return;
        }

        // Get status from database
        var statuses = await _context.Set<Status>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        var status = statuses.FirstOrDefault();
        if (status == null)
        {
            Console.WriteLine("   No statuses found. Cannot create task.\n");
            return;
        }

        // 1. CREATE - Creating a new task
        Console.WriteLine("1. CREATE - Creating a new task:");
        var newTask = new TaskModel
        {
            OwnerId = owner.Id,
            StatusId = status.Id,
            Title = "Demo Task",
            Description = "This is a demonstration task",
            Priority = 1,
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
            UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
            EstimatedHours = 5,
            ActualHours = 0
        };

        var createdTask = await _taskRepository.CreateAsync(newTask, cancellationToken);
        Console.WriteLine($"   Created Task ID: {createdTask.Id}, Title: {createdTask.Title}\n");

        // 2. READ - Reading by ID
        Console.WriteLine("2. READ - Reading task by ID:");
        var taskById = await _taskRepository.GetByIdAsync(createdTask.Id, cancellationToken);
        if (taskById != null)
        {
            Console.WriteLine($"   Found Task ID: {taskById.Id}, Title: {taskById.Title}\n");
        }
        else
        {
            Console.WriteLine("   Task not found\n");
        }

        // 3. READ ALL - Reading all tasks with AsNoTracking (read-only, better performance)
        Console.WriteLine("3. READ ALL - Reading all tasks (AsNoTracking for read-only):");
        var allTasks = await _taskRepository.GetAllAsync(trackChanges: false, cancellationToken);
        Console.WriteLine($"   Total tasks: {allTasks.Count()}\n");

        // 4. UPDATE - Method 1: Tracked entity (automatic change detection)
        Console.WriteLine("4. UPDATE - Method 1: Tracked entity (automatic change detection):");
        Console.WriteLine("   Entity is already tracked from GetByIdAsync()");
        Console.WriteLine("   EF Core automatically detects property changes");
        if (taskById != null)
        {
            // Entity is already tracked from GetByIdAsync
            taskById.Title = "Updated Task Title (Tracked)";
            taskById.Description = "Updated description using tracked entity";
            taskById.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            var updatedTask = await _taskRepository.UpdateAsync(taskById, cancellationToken);
            Console.WriteLine($"   ✓ Updated Task ID: {updatedTask.Id}, New Title: {updatedTask.Title}\n");
        }

        // 5. UPDATE - Method 2: Detached entity (manual tracking)
        Console.WriteLine("5. UPDATE - Method 2: Detached entity (manual tracking):");
        Console.WriteLine("   Entity is not tracked (from GetAllAsync with trackChanges: false)");
        Console.WriteLine("   Must use Update() method to attach and mark as modified");
        
        // Get task without tracking to demonstrate detached approach
        var detachedTasks = await _taskRepository.GetAllAsync(trackChanges: false, cancellationToken);
        var taskToUpdate = detachedTasks.FirstOrDefault(t => t.Id == createdTask.Id);

        if (taskToUpdate != null)
        {
            // Create a new instance with updated values (simulating detached entity from API/DTO)
            var updatedDetachedTask = new TaskModel
            {
                Id = taskToUpdate.Id,
                OwnerId = taskToUpdate.OwnerId,
                StatusId = taskToUpdate.StatusId,
                Title = "Updated Task Title (Detached)",
                Description = "Updated description using detached entity",
                Priority = taskToUpdate.Priority,
                Deadline = taskToUpdate.Deadline,
                CreatedAt = taskToUpdate.CreatedAt,
                UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                EstimatedHours = taskToUpdate.EstimatedHours,
                ActualHours = taskToUpdate.ActualHours,
                CategoryId = taskToUpdate.CategoryId,
                ProjectId = taskToUpdate.ProjectId
            };

            var updatedTask2 = await _taskRepository.UpdateAsync(updatedDetachedTask, cancellationToken);
            Console.WriteLine($"   ✓ Updated Task ID: {updatedTask2.Id}, New Title: {updatedTask2.Title}\n");
        }

        // 6. DELETE - Deleting a task
        Console.WriteLine("6. DELETE - Deleting a task:");
        var deleted = await _taskRepository.DeleteAsync(createdTask.Id, cancellationToken);
        Console.WriteLine($"   Task deleted: {deleted}\n");

        Console.WriteLine("=== Task Repository Demo Completed ===\n");
    }

    /// <summary>
    /// Demonstrates all CRUD operations for User entity
    /// </summary>
    public async Task DemonstrateUserOperationsAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== User Repository CRUD Operations Demo ===\n");

        // 1. CREATE
        Console.WriteLine("1. CREATE - Creating a new user:");
        var newUser = new User
        {
            Name = "Demo",
            Surname = "User",
            Login = $"demo_user_{Guid.NewGuid():N}",
            PasswordHash = "hashed_password",
            Salt = "salt_value",
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };

        var createdUser = await _userRepository.CreateAsync(newUser, cancellationToken);
        Console.WriteLine($"   Created User ID: {createdUser.Id}, Login: {createdUser.Login}\n");

        // 2. READ
        Console.WriteLine("2. READ - Reading user by ID:");
        var userById = await _userRepository.GetByIdAsync(createdUser.Id, cancellationToken);
        if (userById != null)
        {
            Console.WriteLine($"   Found User ID: {userById.Id}, Name: {userById.Name} {userById.Surname}\n");
        }

        // 3. READ ALL with AsNoTracking
        Console.WriteLine("3. READ ALL - Reading all users (AsNoTracking):");
        var allUsers = await _userRepository.GetAllAsync(trackChanges: false, cancellationToken);
        Console.WriteLine($"   Total users: {allUsers.Count()}\n");

        // 4. UPDATE - Tracked entity
        Console.WriteLine("4. UPDATE - Tracked entity:");
        if (userById != null)
        {
            userById.Name = "Updated";
            userById.Surname = "Name";
            var updatedUser = await _userRepository.UpdateAsync(userById, cancellationToken);
            Console.WriteLine($"   Updated User ID: {updatedUser.Id}, New Name: {updatedUser.Name} {updatedUser.Surname}\n");
        }

        // 5. DELETE
        Console.WriteLine("5. DELETE - Deleting user:");
        var deleted = await _userRepository.DeleteAsync(createdUser.Id, cancellationToken);
        Console.WriteLine($"   User deleted: {deleted}\n");

        Console.WriteLine("=== User Repository Demo Completed ===\n");
    }

    /// <summary>
    /// Demonstrates all CRUD operations for Project entity
    /// </summary>
    public async Task DemonstrateProjectOperationsAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== Project Repository CRUD Operations Demo ===\n");

        // Get first user for owner
        var firstUser = await _userRepository.GetAllAsync(trackChanges: false, cancellationToken);
        var owner = firstUser.FirstOrDefault();
        if (owner == null)
        {
            Console.WriteLine("   No users found. Cannot create project.\n");
            return;
        }

        // 1. CREATE
        Console.WriteLine("1. CREATE - Creating a new project:");
        var newProject = new Project
        {
            OwnerId = owner.Id,
            Name = "Demo Project",
            Description = "This is a demonstration project",
            StartDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
            EndDate = DateTime.SpecifyKind(DateTime.UtcNow.AddMonths(3), DateTimeKind.Unspecified)
        };

        var createdProject = await _projectRepository.CreateAsync(newProject, cancellationToken);
        Console.WriteLine($"   Created Project ID: {createdProject.Id}, Name: {createdProject.Name}\n");

        // 2. READ
        Console.WriteLine("2. READ - Reading project by ID:");
        var projectById = await _projectRepository.GetByIdAsync(createdProject.Id, cancellationToken);
        if (projectById != null)
        {
            Console.WriteLine($"   Found Project ID: {projectById.Id}, Name: {projectById.Name}\n");
        }

        // 3. READ ALL with AsNoTracking
        Console.WriteLine("3. READ ALL - Reading all projects (AsNoTracking):");
        var allProjects = await _projectRepository.GetAllAsync(trackChanges: false, cancellationToken);
        Console.WriteLine($"   Total projects: {allProjects.Count()}\n");

        // 4. UPDATE - Tracked entity
        Console.WriteLine("4. UPDATE - Tracked entity:");
        if (projectById != null)
        {
            projectById.Name = "Updated Project Name";
            projectById.Description = "Updated description";
            var updatedProject = await _projectRepository.UpdateAsync(projectById, cancellationToken);
            Console.WriteLine($"   Updated Project ID: {updatedProject.Id}, New Name: {updatedProject.Name}\n");
        }

        // 5. DELETE
        Console.WriteLine("5. DELETE - Deleting project:");
        var deleted = await _projectRepository.DeleteAsync(createdProject.Id, cancellationToken);
        Console.WriteLine($"   Project deleted: {deleted}\n");

        Console.WriteLine("=== Project Repository Demo Completed ===\n");
    }

    /// <summary>
    /// Demonstrates the difference between tracked and detached entities
    /// </summary>
    public async Task DemonstrateChangeTrackingAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== Change Tracking Demonstration ===\n");

        // Get first task
        var allTasks = await _taskRepository.GetAllAsync(trackChanges: false, cancellationToken);
        var task = allTasks.FirstOrDefault();
        if (task == null)
        {
            Console.WriteLine("   No tasks found. Cannot demonstrate change tracking.\n");
            return;
        }

        Console.WriteLine("Understanding Change Tracking:\n");

        // Tracked Entity Example
        Console.WriteLine("1. TRACKED ENTITY (Automatic Change Detection):");
        Console.WriteLine("   - Entity is loaded and tracked by DbContext");
        Console.WriteLine("   - EF Core automatically detects property changes");
        Console.WriteLine("   - No need to call Update() method");
        Console.WriteLine("   - Example: GetByIdAsync() returns tracked entity\n");

        var trackedTask = await _taskRepository.GetByIdAsync(task.Id, cancellationToken);
        if (trackedTask != null)
        {
            Console.WriteLine($"   Original Title: {trackedTask.Title}");
            trackedTask.Title = "Modified by Tracked Entity";
            Console.WriteLine($"   Modified Title: {trackedTask.Title}");
            Console.WriteLine("   - Changes are automatically detected when SaveChangesAsync() is called\n");
        }

        // Detached Entity Example
        Console.WriteLine("2. DETACHED ENTITY (Manual Tracking):");
        Console.WriteLine("   - Entity is not tracked by DbContext");
        Console.WriteLine("   - Must use Update() method to attach and mark as modified");
        Console.WriteLine("   - Example: GetAllAsync(trackChanges: false) returns detached entities");
        Console.WriteLine("   - Useful when entity comes from outside (API, DTO, etc.)\n");

        var detachedTasks = await _taskRepository.GetAllAsync(trackChanges: false, cancellationToken);
        var detachedTask = detachedTasks.FirstOrDefault(t => t.Id == task.Id);
        if (detachedTask != null)
        {
            Console.WriteLine($"   Original Title: {detachedTask.Title}");
            var modifiedTask = new TaskModel
            {
                Id = detachedTask.Id,
                OwnerId = detachedTask.OwnerId,
                StatusId = detachedTask.StatusId,
                Title = "Modified by Detached Entity",
                Description = detachedTask.Description,
                Priority = detachedTask.Priority,
                Deadline = detachedTask.Deadline,
                CreatedAt = detachedTask.CreatedAt,
                UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                EstimatedHours = detachedTask.EstimatedHours,
                ActualHours = detachedTask.ActualHours,
                CategoryId = detachedTask.CategoryId,
                ProjectId = detachedTask.ProjectId
            };
            Console.WriteLine($"   Modified Title: {modifiedTask.Title}");
            Console.WriteLine("   - Must use Update() to attach entity to context\n");
        }

        // AsNoTracking Benefits
        Console.WriteLine("3. AsNoTracking() Benefits:");
        Console.WriteLine("   - Improves performance for read-only operations");
        Console.WriteLine("   - Reduces memory usage (no change tracking overhead)");
        Console.WriteLine("   - Faster query execution");
        Console.WriteLine("   - Use when you don't need to modify entities\n");

        Console.WriteLine("=== Change Tracking Demo Completed ===\n");
    }

    /// <summary>
    /// Runs all demonstrations
    /// </summary>
    public async Task RunAllDemonstrationsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await DemonstrateChangeTrackingAsync(cancellationToken);
            await DemonstrateTaskOperationsAsync(cancellationToken);
            await DemonstrateUserOperationsAsync(cancellationToken);
            await DemonstrateProjectOperationsAsync(cancellationToken);

            Console.WriteLine("=== All Demonstrations Completed Successfully ===");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during demonstration: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}

