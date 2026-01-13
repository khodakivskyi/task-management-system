namespace backend.GraphQL.Tasks.Inputs;

/// <summary>
/// Input type for deleting a task
/// </summary>
public class DeleteTaskInput
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
}
