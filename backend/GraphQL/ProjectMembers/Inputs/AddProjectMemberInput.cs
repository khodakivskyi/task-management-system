namespace backend.GraphQL.ProjectMembers.Inputs;

/// <summary>
/// Input type for adding a project member
/// </summary>
public class AddProjectMemberInput
{
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public int RequestingUserId { get; set; }
}
