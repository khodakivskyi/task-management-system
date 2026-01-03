namespace backend.GraphQL.Projects.Inputs;

/// <summary>
/// Input type for creating a new project
/// </summary>
public class CreateProjectInput
{
    public int OwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
