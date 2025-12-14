using backend.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.CodeFirst.Configurations;

/// <summary>
/// Code-First configuration for Comment entity using Fluent API
/// Replaces all data annotations from scaffolded entity
/// </summary>
public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        // Primary key
        builder.HasKey(c => c.Id);

        // Properties configuration
        builder.Property(c => c.TaskId)
            .IsRequired();

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(c => c.Content)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Table name
        builder.ToTable("Comments");

        // Indexes
        builder.HasIndex(c => c.TaskId)
            .HasDatabaseName("IX_Comments_TaskId");

        builder.HasIndex(c => c.UserId)
            .HasDatabaseName("IX_Comments_UserId");

        builder.HasIndex(c => c.CreatedAt)
            .HasDatabaseName("IX_Comments_CreatedAt");

        // Composite indexes
        builder.HasIndex(c => new { c.TaskId, c.CreatedAt })
            .HasDatabaseName("IX_Comments_TaskId_CreatedAt");

        // Foreign keys and relationships
        // Comment -> Task
        builder.HasOne(c => c.Task)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        // Comment -> User
        builder.HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

