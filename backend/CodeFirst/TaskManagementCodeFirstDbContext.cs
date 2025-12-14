using backend.CodeFirst.Entities;
using backend.CodeFirst.Configurations;
using Microsoft.EntityFrameworkCore;
using TaskEntity = backend.CodeFirst.Entities.Task;

namespace backend.CodeFirst;

/// <summary>
/// Code-First DbContext created from scratch
/// NOT copied from scaffolded DbContext
/// Uses Fluent API configurations through IEntityTypeConfiguration<T>
/// </summary>
public class TaskManagementCodeFirstDbContext : DbContext
{
    /// <summary>
    /// Constructor that accepts DbContextOptions
    /// Connection string is configured through DI, NOT in OnConfiguring
    /// </summary>
    public TaskManagementCodeFirstDbContext(DbContextOptions<TaskManagementCodeFirstDbContext> options)
        : base(options)
    {
    }

    // DbSet properties for each entity
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<EntityType> EntityTypes => Set<EntityType>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<ProjectRole> ProjectRoles => Set<ProjectRole>();
    public DbSet<Status> Statuses => Set<Status>();
    public DbSet<TaskEntity> Tasks => Set<TaskEntity>();
    public DbSet<TaskAssignee> TaskAssignees => Set<TaskAssignee>();
    public DbSet<TaskHistory> TaskHistories => Set<TaskHistory>();
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// OnModelCreating method to apply all entity configurations
    /// Uses ApplyConfigurationsFromAssembly to automatically load all IEntityTypeConfiguration<T>
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the Configurations assembly
        // This automatically loads all classes that implement IEntityTypeConfiguration<T>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskManagementCodeFirstDbContext).Assembly);

        // Alternative approach (manual registration):
        // modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        // modelBuilder.ApplyConfiguration(new StatusConfiguration());
        // ... etc
    }

    // NOTE: OnConfiguring is NOT used for connection string
    // Connection string is provided through DbContextOptions in constructor
    // This is configured in Program.cs via dependency injection
}

