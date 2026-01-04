using backend.Models;
using backend.Services.Interfaces;

namespace backend.GraphQL.Queries;

/// <summary>
/// GraphQL Query operations for ProjectMembers
/// </summary>
public class ProjectMembersQuery
{
    public async Task<IEnumerable<ProjectMember>> GetProjectMembers(
        int projectId,
        [Service] IProjectMemberService projectMemberService)
    {
        return await projectMemberService.GetProjectMembersAsync(projectId);
    }
}
