using backend.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.CodeFirst.Configurations;

/// <summary>
/// Code-First configuration for TaskAssignee entity using Fluent API
/// Represents many-to-many relationship between Task and User
/// Replaces all data annotations from scaffolded entity
/// </summary>
public class TaskAssigneeConfiguration : IEntityTypeConfiguration<TaskAssignee>
{
    public void Configure(EntityTypeBuilder<TaskAssignee> builder)
    {
        // Primary key
        builder.HasKey(ta => ta.Id);

        // Properties configuration
        builder.Property(ta => ta.TaskId)
            .IsRequired();

        builder.Property(ta => ta.UserId)
            .IsRequired();

        // Table name
        builder.ToTable("TaskAssignees");

        // Indexes
        builder.HasIndex(ta => ta.TaskId)
            .HasDatabaseName("IX_TaskAssignees_TaskId");

        builder.HasIndex(ta => ta.UserId)
            .HasDatabaseName("IX_TaskAssignees_UserId");

        // Composite indexes
        builder.HasIndex(ta => new { ta.TaskId, ta.UserId })
            .IsUnique()
            .HasDatabaseName("IX_TaskAssignees_TaskId_UserId");

        builder.HasIndex(ta => new { ta.UserId, ta.TaskId })
            .HasDatabaseName("IX_TaskAssignees_UserId_TaskId");

        // Foreign keys and relationships
        // TaskAssignee -> Task
        builder.HasOne(ta => ta.Task)
            .WithMany(t => t.TaskAssignees)
            .HasForeignKey(ta => ta.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        // TaskAssignee -> User
        builder.HasOne(ta => ta.User)
            .WithMany(u => u.TaskAssignees)
            .HasForeignKey(ta => ta.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

