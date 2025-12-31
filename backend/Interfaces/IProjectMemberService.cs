using backend.Models;

namespace backend.Interfaces;

/// <summary>
/// Service interface for ProjectMember operations
/// </summary>
public interface IProjectMemberService
{
    Task<ProjectMember> AddMemberAsync(int projectId, int userId, int roleId);
    Task RemoveMemberAsync(int projectId, int userId);
    Task<ProjectMember> UpdateMemberRoleAsync(int projectId, int userId, int newRoleId);
    Task<IEnumerable<ProjectMember>> GetProjectMembersAsync(int projectId);
}
