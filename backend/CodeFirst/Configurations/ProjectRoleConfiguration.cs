using backend.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.CodeFirst.Configurations;

/// <summary>
/// Code-First configuration for ProjectRole entity (lookup table)
/// Implements IEntityTypeConfiguration<T> pattern
/// Configures: primary keys, properties, seed data
/// </summary>
public class ProjectRoleConfiguration : IEntityTypeConfiguration<ProjectRole>
{
    public void Configure(EntityTypeBuilder<ProjectRole> builder)
    {
        // Table name
        builder.ToTable("ProjectRoles");

        // Primary key
        builder.HasKey(pr => pr.Id);

        // Properties configuration
        builder.Property(pr => pr.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(pr => pr.CanCreateTasks)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(pr => pr.CanEditTasks)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(pr => pr.CanDeleteTasks)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(pr => pr.CanAssignTasks)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(pr => pr.CanManageMembers)
            .IsRequired()
            .HasDefaultValue(false);

        // Seed data - Lookup table (no foreign keys, can be seeded first)
        builder.HasData(
            new ProjectRole
            {
                Id = 1,
                Name = "Owner",
                CanCreateTasks = true,
                CanEditTasks = true,
                CanDeleteTasks = true,
                CanAssignTasks = true,
                CanManageMembers = true
            },
            new ProjectRole
            {
                Id = 2,
                Name = "Manager",
                CanCreateTasks = true,
                CanEditTasks = true,
                CanDeleteTasks = true,
                CanAssignTasks = true,
                CanManageMembers = false
            },
            new ProjectRole
            {
                Id = 3,
                Name = "Developer",
                CanCreateTasks = true,
                CanEditTasks = true,
                CanDeleteTasks = false,
                CanAssignTasks = false,
                CanManageMembers = false
            },
            new ProjectRole
            {
                Id = 4,
                Name = "Viewer",
                CanCreateTasks = false,
                CanEditTasks = false,
                CanDeleteTasks = false,
                CanAssignTasks = false,
                CanManageMembers = false
            }
        );
    }
}

