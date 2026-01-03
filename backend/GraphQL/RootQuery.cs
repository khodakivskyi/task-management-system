using backend.GraphQL.Queries;

namespace backend.GraphQL;

public class RootQuery
{
    public TasksQuery Tasks() => new();
}
