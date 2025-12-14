using backend.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.CodeFirst.Configurations;

/// <summary>
/// Code-First configuration for Category entity using Fluent API
/// Replaces all data annotations from scaffolded entity
/// </summary>
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // Primary key (convention-based, but explicit for clarity)
        builder.HasKey(c => c.Id);

        // Properties configuration
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Color)
            .IsRequired()
            .HasMaxLength(7); // Hex color format: #RRGGBB

        // Table name (optional, follows convention)
        builder.ToTable("Categories");

        // Navigation properties are configured automatically by convention
        // One-to-many: Category -> Tasks
        builder.HasMany(c => c.Tasks)
            .WithOne(t => t.Category)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.SetNull); // If category is deleted, set CategoryId to null
    }
}

