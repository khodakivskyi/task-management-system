using backend.EFModels;
using backend.EFModels.Enums;
using backend.EFModels.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace backend;

/// <summary>
/// Partial class for TaskManagementDbContext to configure advanced EF Core patterns
/// - Global Query Filters for Soft Delete
/// - Value Converters (Enum to String, JSON serialization)
/// - Owned Entities (Value Objects)
/// </summary>
public partial class TaskManagementDbContext
{
    /// <summary>
    /// Configures advanced EF Core patterns in OnModelCreating
    /// Called automatically by EF Core through partial method
    /// </summary>
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        ConfigureSoftDelete(modelBuilder);
        ConfigureValueConverters(modelBuilder);
        ConfigureOwnedEntities(modelBuilder);
    }

    /// <summary>
    /// Configures global query filter for soft delete on TaskModel, User, and Project
    /// All queries automatically exclude deleted entities (IsDeleted = true)
    /// Use IgnoreQueryFilters() to include deleted entities
    /// </summary>
    private void ConfigureSoftDelete(ModelBuilder modelBuilder)
    {
        // Global query filter for TaskModel
        modelBuilder.Entity<TaskModel>(entity =>
        {
            // Global query filter - automatically filters out deleted entities
            entity.HasQueryFilter(t => !t.IsDeleted);

            // Configure IsDeleted column
            entity.Property(t => t.IsDeleted)
                .HasDefaultValue(false)
                .IsRequired();

            // Configure DeletedAt column
            entity.Property(t => t.DeletedAt)
                .IsRequired(false)
                .HasColumnType("timestamp with time zone");
        });

        // Global query filter for User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasQueryFilter(u => !u.IsDeleted);

            entity.Property(u => u.IsDeleted)
                .HasDefaultValue(false)
                .IsRequired();

            entity.Property(u => u.DeletedAt)
                .IsRequired(false)
                .HasColumnType("timestamp without time zone");
        });

        // Global query filter for Project
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasQueryFilter(p => !p.IsDeleted);

            entity.Property(p => p.IsDeleted)
                .HasDefaultValue(false)
                .IsRequired();

            entity.Property(p => p.DeletedAt)
                .IsRequired(false)
                .HasColumnType("timestamp without time zone");
        });
    }

    /// <summary>
    /// Configures value converters for TaskModel
    /// - Enum to String converter for PriorityLevel
    /// - JSON converter for Metadata
    /// </summary>
    private void ConfigureValueConverters(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskModel>(entity =>
        {
            // Value converter: Enum to String
            // Stores TaskPriorityLevel enum as string ("Low", "Medium", "High", "Critical")
            // instead of integer (1, 2, 3, 4)
            entity.Property(t => t.PriorityLevel)
                .HasConversion<string>()
                .IsRequired(false);

            // Value converter: JSON serialization
            // Stores TaskMetadata complex object as JSON string
            entity.Property(t => t.Metadata)
                .HasConversion(
                    // Convert to JSON string when saving to database
                    v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    // Convert from JSON string when reading from database
                    v => string.IsNullOrEmpty(v) ? null : JsonSerializer.Deserialize<TaskMetadata>(v, (JsonSerializerOptions?)null))
                .IsRequired(false)
                .HasColumnType("text"); // Store as text column
        });
    }

    /// <summary>
    /// Configures owned entities (value objects) for Project
    /// Address value object is stored in the same table as Project
    /// </summary>
    private void ConfigureOwnedEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(entity =>
        {
            // Configure Address as owned entity
            // EF Core will store Address properties in the same table as Project
            // Column names: Address_Street, Address_City, Address_ZipCode, Address_Country
            entity.OwnsOne(p => p.Address, address =>
            {
                address.Property(a => a.Street)
                    .HasColumnName("Address_Street")
                    .HasMaxLength(255);

                address.Property(a => a.City)
                    .HasColumnName("Address_City")
                    .HasMaxLength(100);

                address.Property(a => a.ZipCode)
                    .HasColumnName("Address_ZipCode")
                    .HasMaxLength(20);

                address.Property(a => a.Country)
                    .HasColumnName("Address_Country")
                    .HasMaxLength(100);
            });
        });
    }
}

