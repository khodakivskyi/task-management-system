namespace backend.GraphQL.Projects.Inputs;

/// <summary>
/// Input type for deleting a project
/// </summary>
public class DeleteProjectInput
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
}
