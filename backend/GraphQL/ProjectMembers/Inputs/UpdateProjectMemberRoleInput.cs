namespace backend.GraphQL.ProjectMembers.Inputs;

/// <summary>
/// Input type for updating a project member role
/// </summary>
public class UpdateProjectMemberRoleInput
{
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public int NewRoleId { get; set; }
}
