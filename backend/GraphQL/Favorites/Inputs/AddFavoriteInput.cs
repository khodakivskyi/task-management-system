namespace backend.GraphQL.Favorites.Inputs;

/// <summary>
/// Input type for adding a favorite
/// </summary>
public class AddFavoriteInput
{
    public int UserId { get; set; }
    public int EntityTypeId { get; set; }
    public int EntityId { get; set; }
}
