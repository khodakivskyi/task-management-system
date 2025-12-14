using backend.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.CodeFirst.Configurations;

/// <summary>
/// Code-First configuration for Project entity using Fluent API
/// Replaces all data annotations from scaffolded entity
/// </summary>
public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        // Primary key
        builder.HasKey(p => p.Id);

        // Properties configuration
        builder.Property(p => p.OwnerId)
            .IsRequired();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.StartDate)
            .IsRequired()
            .HasColumnType("timestamp without time zone");

        builder.Property(p => p.EndDate)
            .IsRequired()
            .HasColumnType("timestamp without time zone");

        // Table name
        builder.ToTable("Projects");

        // Indexes
        builder.HasIndex(p => p.Name)
            .HasDatabaseName("IX_Projects_Name");

        builder.HasIndex(p => p.OwnerId)
            .HasDatabaseName("IX_Projects_OwnerId");

        builder.HasIndex(p => p.StartDate)
            .HasDatabaseName("IX_Projects_StartDate");

        builder.HasIndex(p => p.EndDate)
            .HasDatabaseName("IX_Projects_EndDate");

        // Foreign keys and relationships
        // Project -> Owner (User)
        builder.HasOne(p => p.Owner)
            .WithMany(u => u.Projects)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Project -> ProjectMembers (many-to-many through join table)
        builder.HasMany(p => p.ProjectMembers)
            .WithOne(pm => pm.Project)
            .HasForeignKey(pm => pm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Project -> Tasks
        builder.HasMany(p => p.Tasks)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

