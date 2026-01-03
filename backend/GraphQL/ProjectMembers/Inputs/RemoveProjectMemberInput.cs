namespace backend.GraphQL.ProjectMembers.Inputs;

/// <summary>
/// Input type for removing a project member
/// </summary>
public class RemoveProjectMemberInput
{
    public int ProjectId { get; set; }
    public int UserId { get; set; }
}
