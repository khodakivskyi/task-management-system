using backend.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.CodeFirst.Configurations;

/// <summary>
/// Code-First configuration for User entity using Fluent API
/// Replaces all data annotations from scaffolded entity
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
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
            .HasMaxLength(255);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Salt)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Table name
        builder.ToTable("Users");

        // Indexes
        builder.HasIndex(u => u.Login)
            .IsUnique()
            .HasDatabaseName("IX_Users_Login");

        builder.HasIndex(u => u.Name)
            .HasDatabaseName("IX_Users_Name");

        builder.HasIndex(u => u.Surname)
            .HasDatabaseName("IX_Users_Surname");

        builder.HasIndex(u => new { u.Name, u.Surname })
            .HasDatabaseName("IX_Users_Name_Surname");

        // Navigation properties - One-to-many relationships
        // User -> Tasks (as Owner)
        builder.HasMany(u => u.Tasks)
            .WithOne(t => t.Owner)
            .HasForeignKey(t => t.OwnerId)
            .OnDelete(DeleteBehavior.Restrict); // Cannot delete user if they own tasks

        // User -> Projects (as Owner)
        builder.HasMany(u => u.Projects)
            .WithOne(p => p.Owner)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict); // Cannot delete user if they own projects

        // User -> Comments
        builder.HasMany(u => u.Comments)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // User -> TaskAssignees (many-to-many through join table)
        builder.HasMany(u => u.TaskAssignees)
            .WithOne(ta => ta.User)
            .HasForeignKey(ta => ta.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // User -> TaskHistories
        builder.HasMany(u => u.TaskHistories)
            .WithOne(th => th.User)
            .HasForeignKey(th => th.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // User -> ProjectMembers (many-to-many through join table)
        builder.HasMany(u => u.ProjectMembers)
            .WithOne(pm => pm.User)
            .HasForeignKey(pm => pm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // User -> Favorites
        builder.HasMany(u => u.Favorites)
            .WithOne(f => f.User)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

