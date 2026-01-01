using backend.Exceptions;
using backend.Interfaces;
using backend.Models;

namespace backend.Helpers;

/// <summary>
/// Validation helper for ProjectMember entity
/// </summary>
public static class ProjectMemberHelper
{
    /// <summary>
    /// Validates project member data
    /// </summary>
    public static async Task ValidateProjectMemberAsync(
        int projectId,
        int userId,
        int roleId,
        IRepository<Project> projectRepository,
        IRepository<User> userRepository,
        IRepository<ProjectRole>? projectRoleRepository = null)
    {
        // Validate ProjectId exists
        var project = await projectRepository.GetByIdAsync(projectId);
        if (project == null)
        {
            throw new NotFoundException($"Project with id {projectId} not found");
        }

        // Validate UserId exists
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException($"User with id {userId} not found");
        }

        // Validate RoleId exists if repository is provided
        if (projectRoleRepository != null)
        {
            var role = await projectRoleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                throw new NotFoundException($"Project role with id {roleId} not found");
            }
        }
    }
}
