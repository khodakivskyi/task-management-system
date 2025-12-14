using backend.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskEntity = backend.CodeFirst.Entities.Task;

namespace backend.CodeFirst.Configurations;

/// <summary>
/// Code-First configuration for Task entity using Fluent API
/// Implements IEntityTypeConfiguration<T> pattern
/// Configures: primary keys, properties, indexes, relationships
/// </summary>
public class TaskConfiguration : IEntityTypeConfiguration<TaskEntity>
{
    public void Configure(EntityTypeBuilder<TaskEntity> builder)
    {
        // Table name
        builder.ToTable("Tasks");

        // Primary key
        builder.HasKey(t => t.Id);

        // Properties configuration
        builder.Property(t => t.OwnerId)
            .IsRequired();

        builder.Property(t => t.ProjectId)
            .IsRequired(false);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.Property(t => t.Priority)
            .IsRequired(false);

        builder.Property(t => t.Deadline)
            .IsRequired(false)
            .HasColumnType("timestamp without time zone");

        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(t => t.UpdatedAt)
            .IsRequired()
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(t => t.EstimatedHours)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(t => t.ActualHours)
            .IsRequired()
            .HasDefaultValue(0);

        // Computed column: Progress percentage (if EstimatedHours > 0)
        // Note: PostgreSQL syntax for computed columns
        builder.Property(t => t.ProgressPercentage)
            .HasComputedColumnSql(
                "CASE WHEN \"EstimatedHours\" > 0 THEN ROUND((\"ActualHours\"::numeric / \"EstimatedHours\"::numeric * 100.0), 2) ELSE 0.0 END",
                stored: true);

        // Indexes
        // Regular index on OwnerId (foreign key)
        builder.HasIndex(t => t.OwnerId)
            .HasDatabaseName("IX_Tasks_OwnerId");

        // Regular index on ProjectId (foreign key, filtered to exclude NULL)
        builder.HasIndex(t => t.ProjectId)
            .HasDatabaseName("IX_Tasks_ProjectId")
            .HasFilter("\"ProjectId\" IS NOT NULL");

        // Regular index on Title for search operations
        builder.HasIndex(t => t.Title)
            .HasDatabaseName("IX_Tasks_Title");

        // Regular index on Priority (filtered to exclude NULL)
        builder.HasIndex(t => t.Priority)
            .HasDatabaseName("IX_Tasks_Priority")
            .HasFilter("\"Priority\" IS NOT NULL");

        // Regular index on Deadline (filtered to exclude NULL)
        builder.HasIndex(t => t.Deadline)
            .HasDatabaseName("IX_Tasks_Deadline")
            .HasFilter("\"Deadline\" IS NOT NULL");

        // Regular index on CreatedAt for date range queries
        builder.HasIndex(t => t.CreatedAt)
            .HasDatabaseName("IX_Tasks_CreatedAt");

        // Regular index on UpdatedAt for tracking recent changes
        builder.HasIndex(t => t.UpdatedAt)
            .HasDatabaseName("IX_Tasks_UpdatedAt");

        // Composite index on OwnerId and Priority for user's task priority queries
        builder.HasIndex(t => new { t.OwnerId, t.Priority })
            .HasDatabaseName("IX_Tasks_OwnerId_Priority")
            .HasFilter("\"Priority\" IS NOT NULL");

        // Composite index on ProjectId and Deadline for project deadline queries
        builder.HasIndex(t => new { t.ProjectId, t.Deadline })
            .HasDatabaseName("IX_Tasks_ProjectId_Deadline")
            .HasFilter("\"ProjectId\" IS NOT NULL AND \"Deadline\" IS NOT NULL");

        // Composite index on OwnerId and CreatedAt for user's recent tasks
        builder.HasIndex(t => new { t.OwnerId, t.CreatedAt })
            .HasDatabaseName("IX_Tasks_OwnerId_CreatedAt");

        // Relationships
        // Task -> Owner (User) - Many-to-One
        builder.HasOne(t => t.Owner)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.OwnerId)
            .OnDelete(DeleteBehavior.Restrict); // Cannot delete user if they own tasks

        // Task -> Project - Many-to-One (optional)
        builder.HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.SetNull); // If project is deleted, set ProjectId to NULL

        // Seed data - Tasks (child entities with FK to Users and Projects)
        // IMPORTANT: Users (Id 1, 2, 3) and Projects (Id 1, 2, 3, 4) must be seeded first
        builder.HasData(
            new TaskEntity
            {
                Id = 1,
                OwnerId = 1, // FK to User with Id = 1
                ProjectId = 1, // FK to Project with Id = 1
                Title = "Design Homepage Layout",
                Description = "Create wireframes and mockups for the new homepage design",
                Priority = 3, // High priority
                Deadline = new DateTime(2024, 2, 15, 0, 0, 0, DateTimeKind.Unspecified),
                CreatedAt = new DateTime(2024, 1, 20, 0, 0, 0, DateTimeKind.Unspecified),
                UpdatedAt = new DateTime(2024, 1, 20, 0, 0, 0, DateTimeKind.Unspecified),
                EstimatedHours = 40,
                ActualHours = 0
            },
            new TaskEntity
            {
                Id = 2,
                OwnerId = 1, // FK to User with Id = 1
                ProjectId = 1, // FK to Project with Id = 1
                Title = "Implement Responsive Navigation",
                Description = "Build responsive navigation menu with mobile hamburger",
                Priority = 2, // Medium priority
                Deadline = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Unspecified),
                CreatedAt = new DateTime(2024, 1, 25, 0, 0, 0, DateTimeKind.Unspecified),
                UpdatedAt = new DateTime(2024, 1, 25, 0, 0, 0, DateTimeKind.Unspecified),
                EstimatedHours = 24,
                ActualHours = 8
            },
            new TaskEntity
            {
                Id = 3,
                OwnerId = 1, // FK to User with Id = 1
                ProjectId = 2, // FK to Project with Id = 2
                Title = "Setup iOS Development Environment",
                Description = "Configure Xcode, CocoaPods, and development certificates",
                Priority = 3, // High priority
                Deadline = new DateTime(2024, 2, 10, 0, 0, 0, DateTimeKind.Unspecified),
                CreatedAt = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Unspecified),
                UpdatedAt = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Unspecified),
                EstimatedHours = 16,
                ActualHours = 16
            },
            new TaskEntity
            {
                Id = 4,
                OwnerId = 2, // FK to User with Id = 2
                ProjectId = 3, // FK to Project with Id = 3
                Title = "Export Data from Legacy Database",
                Description = "Create scripts to export all data from SQL Server database",
                Priority = 3, // High priority
                Deadline = new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Unspecified),
                CreatedAt = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Unspecified),
                UpdatedAt = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Unspecified),
                EstimatedHours = 32,
                ActualHours = 0
            },
            new TaskEntity
            {
                Id = 5,
                OwnerId = 2, // FK to User with Id = 2
                ProjectId = 3, // FK to Project with Id = 3
                Title = "Import Data to PostgreSQL",
                Description = "Import exported data into new PostgreSQL database",
                Priority = 3, // High priority
                Deadline = new DateTime(2024, 4, 1, 0, 0, 0, DateTimeKind.Unspecified),
                CreatedAt = new DateTime(2024, 3, 5, 0, 0, 0, DateTimeKind.Unspecified),
                UpdatedAt = new DateTime(2024, 3, 5, 0, 0, 0, DateTimeKind.Unspecified),
                EstimatedHours = 40,
                ActualHours = 0
            },
            new TaskEntity
            {
                Id = 6,
                OwnerId = 3, // FK to User with Id = 3
                ProjectId = 4, // FK to Project with Id = 4
                Title = "Integrate Payment Gateway API",
                Description = "Integrate Stripe payment gateway for processing payments",
                Priority = 3, // High priority
                Deadline = new DateTime(2024, 5, 1, 0, 0, 0, DateTimeKind.Unspecified),
                CreatedAt = new DateTime(2024, 4, 5, 0, 0, 0, DateTimeKind.Unspecified),
                UpdatedAt = new DateTime(2024, 4, 5, 0, 0, 0, DateTimeKind.Unspecified),
                EstimatedHours = 48,
                ActualHours = 12
            },
            new TaskEntity
            {
                Id = 7,
                OwnerId = 3, // FK to User with Id = 3
                ProjectId = null, // Task without project (optional FK)
                Title = "Update Documentation",
                Description = "Update API documentation with new endpoints",
                Priority = 1, // Low priority
                Deadline = new DateTime(2024, 4, 30, 0, 0, 0, DateTimeKind.Unspecified),
                CreatedAt = new DateTime(2024, 4, 10, 0, 0, 0, DateTimeKind.Unspecified),
                UpdatedAt = new DateTime(2024, 4, 10, 0, 0, 0, DateTimeKind.Unspecified),
                EstimatedHours = 8,
                ActualHours = 0
            }
        );
    }
}
