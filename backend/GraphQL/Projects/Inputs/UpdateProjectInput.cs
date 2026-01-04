namespace backend.GraphQL.Projects.Inputs;

/// <summary>
/// Input type for updating an existing project
/// </summary>
public class UpdateProjectInput
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
