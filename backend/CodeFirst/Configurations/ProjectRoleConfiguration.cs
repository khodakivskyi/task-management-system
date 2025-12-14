using backend.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.CodeFirst.Configurations;

/// <summary>
/// Code-First configuration for ProjectRole entity using Fluent API
/// Replaces all data annotations from scaffolded entity
/// </summary>
public class ProjectRoleConfiguration : IEntityTypeConfiguration<ProjectRole>
{
    public void Configure(EntityTypeBuilder<ProjectRole> builder)
    {
        // Primary key
        builder.HasKey(pr => pr.Id);

        // Properties configuration
        builder.Property(pr => pr.Name)
            .IsRequired()
            .HasMaxLength(50);

        // Default values for boolean properties
        builder.Property(pr => pr.CanCreateTasks)
            .HasDefaultValue(false);

        builder.Property(pr => pr.CanEditTasks)
            .HasDefaultValue(false);

        builder.Property(pr => pr.CanDeleteTasks)
            .HasDefaultValue(false);

        builder.Property(pr => pr.CanAssignTasks)
            .HasDefaultValue(false);

        builder.Property(pr => pr.CanManageMembers)
            .HasDefaultValue(false);

        // Table name
        builder.ToTable("ProjectRoles");

        // One-to-many: ProjectRole -> ProjectMembers
        builder.HasMany(pr => pr.ProjectMembers)
            .WithOne(pm => pm.Role)
            .HasForeignKey(pm => pm.RoleId)
            .OnDelete(DeleteBehavior.Restrict); // Role is required, cannot delete if members exist
    }
}

