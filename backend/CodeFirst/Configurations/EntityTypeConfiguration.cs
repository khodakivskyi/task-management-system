using backend.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.CodeFirst.Configurations;

/// <summary>
/// Code-First configuration for EntityType entity using Fluent API
/// Replaces all data annotations from scaffolded entity
/// </summary>
public class EntityTypeConfiguration : IEntityTypeConfiguration<EntityType>
{
    public void Configure(EntityTypeBuilder<EntityType> builder)
    {
        // Primary key
        builder.HasKey(et => et.Id);

        // Properties configuration
        builder.Property(et => et.Name)
            .IsRequired()
            .HasMaxLength(20);

        // Table name
        builder.ToTable("EntityTypes");

        // Unique index on Name
        builder.HasIndex(et => et.Name)
            .IsUnique();

        // One-to-many: EntityType -> Favorites
        builder.HasMany(et => et.Favorites)
            .WithOne(f => f.EntityType)
            .HasForeignKey(f => f.EntityTypeId)
            .OnDelete(DeleteBehavior.Restrict); // EntityType is required, cannot delete if favorites exist
    }
}

