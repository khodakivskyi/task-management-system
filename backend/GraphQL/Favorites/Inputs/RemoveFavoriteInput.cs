namespace backend.GraphQL.Favorites.Inputs;

/// <summary>
/// Input type for removing a favorite
/// </summary>
public class RemoveFavoriteInput
{
    public int UserId { get; set; }
    public int EntityTypeId { get; set; }
    public int EntityId { get; set; }
}
