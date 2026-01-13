using backend.Models;

namespace backend.Services.Interfaces;

/// <summary>
/// Service interface for ProjectMember operations
/// </summary>
public interface IProjectMemberService
{
    Task<ProjectMember> AddMemberAsync(int projectId, int userId, int roleId, int requestingUserId);
    Task RemoveMemberAsync(int projectId, int userId, int requestingUserId);
    Task<ProjectMember> UpdateMemberRoleAsync(int projectId, int userId, int newRoleId, int requestingUserId);
    Task<IEnumerable<ProjectMember>> GetProjectMembersAsync(int projectId);
}
