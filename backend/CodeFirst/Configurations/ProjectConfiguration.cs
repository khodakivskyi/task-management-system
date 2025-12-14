using backend.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.CodeFirst.Configurations;

/// <summary>
/// Code-First configuration for Project entity using Fluent API
/// Implements IEntityTypeConfiguration<T> pattern
/// Configures: primary keys, properties, indexes, relationships
/// </summary>
public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        // Table name
        builder.ToTable("Projects");

        // Primary key
        builder.HasKey(p => p.Id);

        // Properties configuration
        builder.Property(p => p.OwnerId)
            .IsRequired();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        builder.Property(p => p.StartDate)
            .IsRequired()
            .HasColumnType("timestamp without time zone");

        builder.Property(p => p.EndDate)
            .IsRequired()
            .HasColumnType("timestamp without time zone");

        // Computed columns: DurationDays and IsActive
        // NOTE: Temporarily ignored until migration is created and applied
        // Uncomment after running migration that creates these columns
        // builder.Property(p => p.DurationDays)
        //     .HasComputedColumnSql(
        //         "EXTRACT(DAY FROM (\"EndDate\" - \"StartDate\"))::integer",
        //         stored: true);
        // builder.Property(p => p.IsActive)
        //     .HasComputedColumnSql(
        //         "(CURRENT_DATE >= \"StartDate\"::date AND CURRENT_DATE <= \"EndDate\"::date)",
        //         stored: true);
        
        // Temporarily ignore computed properties until migration is applied
        builder.Ignore(p => p.DurationDays);
        builder.Ignore(p => p.IsActive);

        // Indexes
        // Regular index on OwnerId (foreign key)
        builder.HasIndex(p => p.OwnerId)
            .HasDatabaseName("IX_Projects_OwnerId");

        // Regular index on Name for search operations
        builder.HasIndex(p => p.Name)
            .HasDatabaseName("IX_Projects_Name");

        // Regular index on StartDate for date range queries
        builder.HasIndex(p => p.StartDate)
            .HasDatabaseName("IX_Projects_StartDate");

        // Regular index on EndDate for date range queries
        builder.HasIndex(p => p.EndDate)
            .HasDatabaseName("IX_Projects_EndDate");

        // Composite index on OwnerId and StartDate for user's projects by start date
        builder.HasIndex(p => new { p.OwnerId, p.StartDate })
            .HasDatabaseName("IX_Projects_OwnerId_StartDate");

        // Composite index on StartDate and EndDate for active projects queries
        builder.HasIndex(p => new { p.StartDate, p.EndDate })
            .HasDatabaseName("IX_Projects_StartDate_EndDate");

        // Relationships
        // Project -> Owner (User) - Many-to-One
        builder.HasOne(p => p.Owner)
            .WithMany(u => u.Projects)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict); // Cannot delete user if they own projects

        // Project -> Tasks - One-to-Many
        builder.HasMany(p => p.Tasks)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.SetNull); // If project is deleted, set ProjectId to NULL in tasks
    }
}
