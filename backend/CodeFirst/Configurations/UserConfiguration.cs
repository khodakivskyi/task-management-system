using backend.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.CodeFirst.Configurations;

/// <summary>
/// Code-First configuration for User entity using Fluent API
/// Implements IEntityTypeConfiguration<T> pattern
/// Configures: primary keys, properties, indexes, relationships
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table name
        builder.ToTable("Users");

        // Primary key
        builder.HasKey(u => u.Id);

        // Properties configuration
        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Surname)
            .HasMaxLength(255);

        builder.Property(u => u.Login)
            .IsRequired()
            .HasMaxLength(255)
            .IsUnicode(false); // Login typically doesn't need Unicode

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255)
            .IsUnicode(false);

        builder.Property(u => u.Salt)
            .IsRequired()
            .HasMaxLength(255)
            .IsUnicode(false);

        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Indexes
        // Unique index on Login (filtered to exclude NULL, though Login is required)
        builder.HasIndex(u => u.Login)
            .IsUnique()
            .HasDatabaseName("IX_Users_Login")
            .HasFilter("\"Login\" IS NOT NULL");

        // Regular index on Name for search operations
        builder.HasIndex(u => u.Name)
            .HasDatabaseName("IX_Users_Name");

        // Regular index on Surname (filtered to exclude NULL)
        builder.HasIndex(u => u.Surname)
            .HasDatabaseName("IX_Users_Surname")
            .HasFilter("\"Surname\" IS NOT NULL");

        // Composite index on Name and Surname for full name searches
        builder.HasIndex(u => new { u.Name, u.Surname })
            .HasDatabaseName("IX_Users_Name_Surname")
            .HasFilter("\"Surname\" IS NOT NULL");

        // Index on CreatedAt for date range queries
        builder.HasIndex(u => u.CreatedAt)
            .HasDatabaseName("IX_Users_CreatedAt");

        // Relationships
        // User -> Tasks (One-to-Many: User owns many Tasks)
        builder.HasMany(u => u.Tasks)
            .WithOne(t => t.Owner)
            .HasForeignKey(t => t.OwnerId)
            .OnDelete(DeleteBehavior.Restrict); // Cannot delete user if they own tasks

        // User -> Projects (One-to-Many: User owns many Projects)
        builder.HasMany(u => u.Projects)
            .WithOne(p => p.Owner)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict); // Cannot delete user if they own projects
    }
}
