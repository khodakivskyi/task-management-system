using backend.CodeFirst.Entities;
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

    // DbSet properties - only User, Task, Project entities
    public DbSet<User> Users => Set<User>();
    public DbSet<TaskEntity> Tasks => Set<TaskEntity>();
    public DbSet<Project> Projects => Set<Project>();

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

    }
}

