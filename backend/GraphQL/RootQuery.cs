using backend.GraphQL.Queries;

namespace backend.GraphQL;

public class RootQuery
{
    public TasksQuery Tasks() => new();
    public ProjectsQuery Projects() => new();
    public CategoriesQuery Categories() => new();
    public CommentsQuery Comments() => new();
    public FavoritesQuery Favorites() => new();
    public EntityTypesQuery EntityTypes() => new();
    public StatusesQuery Statuses() => new();
    public TaskHistoryQuery TaskHistory() => new();
    public ProjectMembersQuery ProjectMembers() => new();
    public ProjectStatisticsQuery ProjectStatistics() => new();
    public TaskSearchQuery TaskSearch() => new();
}
