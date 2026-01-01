using backend.Exceptions;
using backend.Interfaces;
using backend.Models;

namespace backend.Helpers;

/// <summary>
/// Validation helper for Project entity
/// </summary>
public static class ProjectHelper
{
    /// <summary>
    /// Validates a project entity
    /// </summary>
    public static async Task ValidateProjectAsync(
        Project project,
        IRepository<User> userRepository)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(project.Name))
        {
            throw new ValidationException("Project name is required");
        }

        if (project.EndDate < project.StartDate)
        {
            throw new ValidationException("End date must be after start date");
        }

        // Validate OwnerId exists
        var owner = await userRepository.GetByIdAsync(project.OwnerId);
        if (owner == null)
        {
            throw new NotFoundException($"User with id {project.OwnerId} not found");
        }
    }
}
