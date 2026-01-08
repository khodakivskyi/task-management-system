using backend.Exceptions;
using backend.Infrastructure.Repositories.Interfaces;
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
        IUserRepository userRepository,
        IRepository<ProjectRole>? projectRoleRepository = null)
    {
        // Validate ProjectId exists
        await EntityValidationHelper.EnsureEntityExistsAsync(projectId, projectRepository, "Project");

        // Validate UserId exists
        await EntityValidationHelper.EnsureEntityExistsAsync(userId, userRepository, "User");

        // Validate RoleId exists if repository is provided
        if (projectRoleRepository != null)
        {
            await EntityValidationHelper.EnsureEntityExistsAsync(roleId, projectRoleRepository, "Project role");
        }
    }
}
