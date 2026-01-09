using backend.GraphQL.Mutations;

namespace backend.GraphQL;

public class RootMutation
{
    public AuthMutation Auth() => new();
    public TasksMutation Tasks() => new();
    public ProjectsMutation Projects() => new();
    public CategoriesMutation Categories() => new();
    public CommentsMutation Comments() => new();
    public FavoritesMutation Favorites() => new();
    public ProjectMembersMutation ProjectMembers() => new();
}
