using backend.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.CodeFirst.Configurations;

/// <summary>
/// Code-First configuration for Status entity (lookup table)
/// Implements IEntityTypeConfiguration<T> pattern
/// Configures: primary keys, properties, seed data
/// </summary>
public class StatusConfiguration : IEntityTypeConfiguration<Status>
{
    public void Configure(EntityTypeBuilder<Status> builder)
    {
        // Table name
        builder.ToTable("Statuses");

        // Primary key
        builder.HasKey(s => s.Id);

        // Properties configuration
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Color)
            .HasMaxLength(7); // Hex color format: #RRGGBB

        // Seed data - Lookup table (no foreign keys, can be seeded first)
        builder.HasData(
            new Status
            {
                Id = 1,
                Name = "To Do",
                Color = "#FF6B6B" // Red
            },
            new Status
            {
                Id = 2,
                Name = "In Progress",
                Color = "#4ECDC4" // Teal
            },
            new Status
            {
                Id = 3,
                Name = "In Review",
                Color = "#FFE66D" // Yellow
            },
            new Status
            {
                Id = 4,
                Name = "Done",
                Color = "#95E1D3" // Green
            },
            new Status
            {
                Id = 5,
                Name = "Cancelled",
                Color = "#C7C7C7" // Gray
            }
        );
    }
}

