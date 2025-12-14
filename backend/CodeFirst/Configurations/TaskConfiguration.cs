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
        // NOTE: Temporarily ignored until migration is created and applied
        // Uncomment after running migration that creates this column
        // builder.Property(t => t.ProgressPercentage)
        //     .HasComputedColumnSql(
        //         "CASE WHEN \"EstimatedHours\" > 0 THEN ROUND((\"ActualHours\"::numeric / \"EstimatedHours\"::numeric * 100.0), 2) ELSE 0.0 END",
        //         stored: true);
        
        // Temporarily ignore computed property until migration is applied
        builder.Ignore(t => t.ProgressPercentage);

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
    }
}
