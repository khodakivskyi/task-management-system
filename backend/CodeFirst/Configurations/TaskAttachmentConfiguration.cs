using backend.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskEntity = backend.CodeFirst.Entities.Task;

namespace backend.CodeFirst.Configurations;

/// <summary>
/// Code-First configuration for TaskAttachment entity using Fluent API
/// Implements IEntityTypeConfiguration<T> pattern
/// </summary>
public class TaskAttachmentConfiguration : IEntityTypeConfiguration<TaskAttachment>
{
    public void Configure(EntityTypeBuilder<TaskAttachment> builder)
    {
        // Table name
        builder.ToTable("TaskAttachments");

        // Primary key
        builder.HasKey(ta => ta.Id);

        // Properties configuration
        builder.Property(ta => ta.TaskId)
            .IsRequired();

        builder.Property(ta => ta.UploadedById)
            .IsRequired();

        builder.Property(ta => ta.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(ta => ta.FilePath)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(ta => ta.FileSize)
            .IsRequired();

        builder.Property(ta => ta.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ta => ta.UploadedAt)
            .IsRequired()
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Indexes
        builder.HasIndex(ta => ta.TaskId)
            .HasDatabaseName("IX_TaskAttachments_TaskId");

        builder.HasIndex(ta => ta.UploadedById)
            .HasDatabaseName("IX_TaskAttachments_UploadedById");

        builder.HasIndex(ta => ta.UploadedAt)
            .HasDatabaseName("IX_TaskAttachments_UploadedAt");

        // Composite index for common queries
        builder.HasIndex(ta => new { ta.TaskId, ta.UploadedAt })
            .HasDatabaseName("IX_TaskAttachments_TaskId_UploadedAt");

        // Relationships
        // TaskAttachment -> Task - Many-to-One
        builder.HasOne(ta => ta.Task)
            .WithMany() // Task doesn't have navigation property to TaskAttachments (to keep it simple)
            .HasForeignKey(ta => ta.TaskId)
            .OnDelete(DeleteBehavior.Cascade); // If task is deleted, delete all attachments

        // TaskAttachment -> User (UploadedBy) - Many-to-One
        builder.HasOne(ta => ta.UploadedBy)
            .WithMany() // User doesn't have navigation property to TaskAttachments (to keep it simple)
            .HasForeignKey(ta => ta.UploadedById)
            .OnDelete(DeleteBehavior.Restrict); // Cannot delete user if they uploaded attachments
    }
}

