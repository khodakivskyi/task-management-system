using backend.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskEntity = backend.CodeFirst.Entities.Task;

namespace backend.CodeFirst.Configurations;

/// <summary>
/// Code-First configuration for Task entity using Fluent API
/// Replaces all data annotations from scaffolded entity
/// </summary>
public class TaskConfiguration : IEntityTypeConfiguration<TaskEntity>
{
    public void Configure(EntityTypeBuilder<TaskEntity> builder)
    {
        // Primary key
        builder.HasKey(t => t.Id);

        // Properties configuration
        builder.Property(t => t.OwnerId)
            .IsRequired();

        builder.Property(t => t.StatusId)
            .IsRequired();

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Description)
            .HasMaxLength(250);

        builder.Property(t => t.Deadline)
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

        // Table name
        builder.ToTable("Tasks");

        // Indexes
        builder.HasIndex(t => t.CategoryId)
            .HasDatabaseName("IX_Tasks_CategoryId");

        builder.HasIndex(t => t.CreatedAt)
            .HasDatabaseName("IX_Tasks_CreatedAt");

        builder.HasIndex(t => t.Deadline)
            .HasDatabaseName("IX_Tasks_Deadline");

        builder.HasIndex(t => t.OwnerId)
            .HasDatabaseName("IX_Tasks_OwnerId");

        builder.HasIndex(t => t.Priority)
            .HasDatabaseName("IX_Tasks_Priority");

        builder.HasIndex(t => t.ProjectId)
            .HasDatabaseName("IX_Tasks_ProjectId");

        builder.HasIndex(t => t.StatusId)
            .HasDatabaseName("IX_Tasks_StatusId");

        builder.HasIndex(t => t.UpdatedAt)
            .HasDatabaseName("IX_Tasks_UpdatedAt");

        // Composite indexes
        builder.HasIndex(t => new { t.OwnerId, t.StatusId })
            .HasDatabaseName("IX_Tasks_OwnerId_StatusId");

        builder.HasIndex(t => new { t.ProjectId, t.StatusId })
            .HasDatabaseName("IX_Tasks_ProjectId_StatusId");

        // Foreign keys and relationships
        // Task -> Owner (User)
        builder.HasOne(t => t.Owner)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Task -> Status
        builder.HasOne(t => t.Status)
            .WithMany(s => s.Tasks)
            .HasForeignKey(t => t.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        // Task -> Category (optional)
        builder.HasOne(t => t.Category)
            .WithMany(c => c.Tasks)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Task -> Project (optional)
        builder.HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);

        // Task -> Comments
        builder.HasMany(t => t.Comments)
            .WithOne(c => c.Task)
            .HasForeignKey(c => c.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        // Task -> TaskAssignees (many-to-many through join table)
        builder.HasMany(t => t.TaskAssignees)
            .WithOne(ta => ta.Task)
            .HasForeignKey(ta => ta.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        // Task -> TaskHistories
        builder.HasMany(t => t.TaskHistories)
            .WithOne(th => th.Task)
            .HasForeignKey(th => th.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

