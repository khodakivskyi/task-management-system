namespace backend.CodeFirst.Entities;

/// <summary>
/// Code-First entity: Favorite
/// Clean entity class without scaffolding annotations
/// Configuration is done through Fluent API in FavoriteConfiguration
/// </summary>
public class Favorite
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int EntityId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int EntityTypeId { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public EntityType EntityType { get; set; } = null!;
}

