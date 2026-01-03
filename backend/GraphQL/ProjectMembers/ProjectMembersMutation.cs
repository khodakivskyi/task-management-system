using backend.GraphQL.ProjectMembers.Inputs;
using backend.Models;
using backend.Services.Interfaces;

namespace backend.GraphQL.Queries;

/// <summary>
/// GraphQL Mutation operations for ProjectMembers
/// </summary>
public class ProjectMembersMutation
{
    public async Task<ProjectMember> AddProjectMember(
        AddProjectMemberInput input,
        [Service] IProjectMemberService projectMemberService)
    {
        return await projectMemberService.AddMemberAsync(input.ProjectId, input.UserId, input.RoleId);
    }

    public async Task<ProjectMember> UpdateProjectMemberRole(
        UpdateProjectMemberRoleInput input,
        [Service] IProjectMemberService projectMemberService)
    {
        return await projectMemberService.UpdateMemberRoleAsync(input.ProjectId, input.UserId, input.NewRoleId);
    }

    public async Task<bool> RemoveProjectMember(
        RemoveProjectMemberInput input,
        [Service] IProjectMemberService projectMemberService)
    {
        await projectMemberService.RemoveMemberAsync(input.ProjectId, input.UserId);
        return true;
    }
}
