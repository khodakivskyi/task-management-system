namespace backend.Models;

/// <summary>
/// Represents an entity type that can be favorited (task or project)
/// </summary>
public class EntityType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // "task" or "project"

    public EntityType() { }
}

