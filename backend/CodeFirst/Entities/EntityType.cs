namespace backend.CodeFirst.Entities;

/// <summary>
/// Code-First entity: EntityType
/// Clean entity class without scaffolding annotations
/// Configuration is done through Fluent API in EntityTypeConfiguration
/// </summary>
public class EntityType
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    // Navigation properties
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}

