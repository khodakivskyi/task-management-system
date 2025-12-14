using backend.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.CodeFirst.Configurations;

/// <summary>
/// Code-First configuration for ProjectMember entity using Fluent API
/// Represents many-to-many relationship between Project and User with additional Role
/// Replaces all data annotations from scaffolded entity
/// </summary>
public class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
{
    public void Configure(EntityTypeBuilder<ProjectMember> builder)
    {
        // Primary key
        builder.HasKey(pm => pm.Id);

        // Properties configuration
        builder.Property(pm => pm.ProjectId)
            .IsRequired();

        builder.Property(pm => pm.UserId)
            .IsRequired();

        builder.Property(pm => pm.RoleId)
            .IsRequired();

        builder.Property(pm => pm.JoinedAt)
            .IsRequired()
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Table name
        builder.ToTable("ProjectMembers");

        // Indexes
        builder.HasIndex(pm => pm.ProjectId)
            .HasDatabaseName("IX_ProjectMembers_ProjectId");

        builder.HasIndex(pm => pm.UserId)
            .HasDatabaseName("IX_ProjectMembers_UserId");

        builder.HasIndex(pm => pm.RoleId)
            .HasDatabaseName("IX_ProjectMembers_RoleId");

        // Composite indexes
        builder.HasIndex(pm => new { pm.ProjectId, pm.UserId })
            .IsUnique()
            .HasDatabaseName("IX_ProjectMembers_ProjectId_UserId");

        builder.HasIndex(pm => new { pm.UserId, pm.ProjectId })
            .HasDatabaseName("IX_ProjectMembers_UserId_ProjectId");

        builder.HasIndex(pm => new { pm.ProjectId, pm.RoleId })
            .HasDatabaseName("IX_ProjectMembers_ProjectId_RoleId");

        // Foreign keys and relationships
        // ProjectMember -> Project
        builder.HasOne(pm => pm.Project)
            .WithMany(p => p.ProjectMembers)
            .HasForeignKey(pm => pm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // ProjectMember -> User
        builder.HasOne(pm => pm.User)
            .WithMany(u => u.ProjectMembers)
            .HasForeignKey(pm => pm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ProjectMember -> ProjectRole
        builder.HasOne(pm => pm.Role)
            .WithMany(pr => pr.ProjectMembers)
            .HasForeignKey(pm => pm.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

