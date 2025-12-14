using backend.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.CodeFirst.Configurations;

/// <summary>
/// Code-First configuration for TaskHistory entity using Fluent API
/// Replaces all data annotations from scaffolded entity
/// </summary>
public class TaskHistoryConfiguration : IEntityTypeConfiguration<TaskHistory>
{
    public void Configure(EntityTypeBuilder<TaskHistory> builder)
    {
        // Primary key
        builder.HasKey(th => th.Id);

        // Properties configuration
        builder.Property(th => th.TaskId)
            .IsRequired();

        builder.Property(th => th.UserId)
            .IsRequired();

        builder.Property(th => th.FieldName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(th => th.ChangedAt)
            .IsRequired()
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Table name
        builder.ToTable("TaskHistory");

        // Indexes
        builder.HasIndex(th => th.TaskId)
            .HasDatabaseName("IX_TaskHistory_TaskId");

        builder.HasIndex(th => th.UserId)
            .HasDatabaseName("IX_TaskHistory_UserId");

        builder.HasIndex(th => th.FieldName)
            .HasDatabaseName("IX_TaskHistory_FieldName");

        builder.HasIndex(th => th.ChangedAt)
            .HasDatabaseName("IX_TaskHistory_ChangedAt");

        // Composite indexes
        builder.HasIndex(th => new { th.TaskId, th.ChangedAt })
            .HasDatabaseName("IX_TaskHistory_TaskId_ChangedAt");

        builder.HasIndex(th => new { th.TaskId, th.UserId })
            .HasDatabaseName("IX_TaskHistory_TaskId_UserId");

        // Foreign keys and relationships
        // TaskHistory -> Task
        builder.HasOne(th => th.Task)
            .WithMany(t => t.TaskHistories)
            .HasForeignKey(th => th.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        // TaskHistory -> User
        builder.HasOne(th => th.User)
            .WithMany(u => u.TaskHistories)
            .HasForeignKey(th => th.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

