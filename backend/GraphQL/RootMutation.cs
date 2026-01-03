using backend.GraphQL.Queries;

namespace backend.GraphQL;

public class RootMutation
{
    public TasksMutation Tasks() => new();
    public ProjectsMutation Projects() => new();
    public CategoriesMutation Categories() => new();
    public CommentsMutation Comments() => new();
    public FavoritesMutation Favorites() => new();
    public ProjectMembersMutation ProjectMembers() => new();
}
