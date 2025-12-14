using backend.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.CodeFirst.Configurations;

/// <summary>
/// Code-First configuration for Status entity using Fluent API
/// Replaces all data annotations from scaffolded entity
/// </summary>
public class StatusConfiguration : IEntityTypeConfiguration<Status>
{
    public void Configure(EntityTypeBuilder<Status> builder)
    {
        // Primary key
        builder.HasKey(s => s.Id);

        // Properties configuration
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Color)
            .HasMaxLength(7); // Hex color format: #RRGGBB

        // Table name
        builder.ToTable("Statuses");

        // One-to-many: Status -> Tasks
        builder.HasMany(s => s.Tasks)
            .WithOne(t => t.Status)
            .HasForeignKey(t => t.StatusId)
            .OnDelete(DeleteBehavior.Restrict); // Status is required, cannot delete if tasks exist
    }
}

