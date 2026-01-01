using backend.Exceptions;
using backend.Interfaces;
using backend.Models;

namespace backend.Helpers;

/// <summary>
/// Validation helper for Task entity
/// </summary>
public static class TaskHelper
{
    /// <summary>
    /// Validates a task entity
    /// </summary>
    public static async Task ValidateTaskAsync(
        TaskModel task,
        IRepository<User> userRepository,
        IRepository<Status> statusRepository,
        IRepository<Category>? categoryRepository = null,
        IRepository<Project>? projectRepository = null)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(task.Title))
        {
            throw new ValidationException("Task title is required");
        }

        if (task.Priority.HasValue && (task.Priority < 1 || task.Priority > 5))
        {
            throw new ValidationException("Priority must be between 1 and 5");
        }

        // Validate OwnerId exists
        await EntityValidationHelper.EnsureEntityExistsAsync(task.OwnerId, userRepository, "User");

        // Validate StatusId exists
        await EntityValidationHelper.EnsureEntityExistsAsync(task.StatusId, statusRepository, "Status");

        // Validate CategoryId if provided
        await EntityValidationHelper.EnsureEntityExistsIfProvidedAsync(task.CategoryId, categoryRepository, "Category");

        // Validate ProjectId if provided
        await EntityValidationHelper.EnsureEntityExistsIfProvidedAsync(task.ProjectId, projectRepository, "Project");
    }
}
