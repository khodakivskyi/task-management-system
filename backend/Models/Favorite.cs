namespace backend.Models;

/// <summary>
/// Represents a favorite item (task or project) for a user
/// </summary>
public class Favorite
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int EntityTypeId { get; set; }
    public int EntityId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public EntityType? EntityType { get; set; }

    public Favorite() { }
}

