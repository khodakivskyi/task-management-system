using backend.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.CodeFirst.Configurations;

/// <summary>
/// Code-First configuration for Category entity (lookup table)
/// Implements IEntityTypeConfiguration<T> pattern
/// Configures: primary keys, properties, seed data
/// </summary>
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // Table name
        builder.ToTable("Categories");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties configuration
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Color)
            .IsRequired()
            .HasMaxLength(7); // Hex color format: #RRGGBB

        // Seed data - Lookup table (no foreign keys, can be seeded first)
        builder.HasData(
            new Category
            {
                Id = 1,
                Name = "Development",
                Color = "#3498DB" // Blue
            },
            new Category
            {
                Id = 2,
                Name = "Design",
                Color = "#9B59B6" // Purple
            },
            new Category
            {
                Id = 3,
                Name = "Testing",
                Color = "#E67E22" // Orange
            },
            new Category
            {
                Id = 4,
                Name = "Documentation",
                Color = "#1ABC9C" // Turquoise
            },
            new Category
            {
                Id = 5,
                Name = "Bug Fix",
                Color = "#E74C3C" // Red
            }
        );
    }
}

