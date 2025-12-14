using backend.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskEntity = backend.CodeFirst.Entities.Task;

namespace backend.CodeFirst.Configurations;

/// <summary>
/// Code-First configuration for TaskComment entity
/// </summary>
public class TaskCommentConfiguration : IEntityTypeConfiguration<TaskComment>
{
    public void Configure(EntityTypeBuilder<TaskComment> builder)
    {
        // Table name
        builder.ToTable("TaskComments");

        // Primary key
        builder.HasKey(tc => tc.Id);

        // Properties configuration
        builder.Property(tc => tc.TaskId)
            .IsRequired();

        builder.Property(tc => tc.UserId)
            .IsRequired();

        builder.Property(tc => tc.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(tc => tc.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(tc => tc.UpdatedAt)
            .HasColumnType("timestamp without time zone");

        // Indexes
        // Regular index on TaskId (foreign key)
        builder.HasIndex(tc => tc.TaskId)
            .HasDatabaseName("IX_TaskComments_TaskId");

        // Regular index on UserId (foreign key)
        builder.HasIndex(tc => tc.UserId)
            .HasDatabaseName("IX_TaskComments_UserId");

        // Regular index on CreatedAt for date range queries
        builder.HasIndex(tc => tc.CreatedAt)
            .HasDatabaseName("IX_TaskComments_CreatedAt");

        // Composite index on TaskId and CreatedAt for task comments ordered by date
        builder.HasIndex(tc => new { tc.TaskId, tc.CreatedAt })
            .HasDatabaseName("IX_TaskComments_TaskId_CreatedAt");

        // Relationships
        // TaskComment -> Task - Many-to-One
        builder.HasOne(tc => tc.Task)
            .WithMany() // Task doesn't have navigation property to comments yet
            .HasForeignKey(tc => tc.TaskId)
            .OnDelete(DeleteBehavior.Cascade); // If task is deleted, delete all comments

        // TaskComment -> User - Many-to-One
        builder.HasOne(tc => tc.User)
            .WithMany() // User doesn't have navigation property to comments yet
            .HasForeignKey(tc => tc.UserId)
            .OnDelete(DeleteBehavior.Restrict); // Cannot delete user if they have comments
    }
}

