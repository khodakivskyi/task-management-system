using backend.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.CodeFirst.Configurations;

/// <summary>
/// Code-First configuration for Favorite entity using Fluent API
/// Replaces all data annotations from scaffolded entity
/// </summary>
public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        // Primary key
        builder.HasKey(f => f.Id);

        // Properties configuration
        builder.Property(f => f.UserId)
            .IsRequired();

        builder.Property(f => f.EntityId)
            .IsRequired();

        builder.Property(f => f.EntityTypeId)
            .IsRequired();

        builder.Property(f => f.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Table name
        builder.ToTable("Favorites");

        // Indexes
        builder.HasIndex(f => f.UserId)
            .HasDatabaseName("IX_Favorites_UserId");

        builder.HasIndex(f => f.EntityTypeId)
            .HasDatabaseName("IX_Favorites_EntityTypeId");

        builder.HasIndex(f => f.CreatedAt)
            .HasDatabaseName("IX_Favorites_CreatedAt");

        // Foreign keys and relationships
        // Favorite -> User
        builder.HasOne(f => f.User)
            .WithMany(u => u.Favorites)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Favorite -> EntityType
        builder.HasOne(f => f.EntityType)
            .WithMany(et => et.Favorites)
            .HasForeignKey(f => f.EntityTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

