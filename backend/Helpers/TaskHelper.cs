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
        var owner = await userRepository.GetByIdAsync(task.OwnerId);
        if (owner == null)
        {
            throw new NotFoundException($"User with id {task.OwnerId} not found");
        }

        // Validate StatusId exists
        var status = await statusRepository.GetByIdAsync(task.StatusId);
        if (status == null)
        {
            throw new NotFoundException($"Status with id {task.StatusId} not found");
        }

        // Validate CategoryId if provided
        if (task.CategoryId.HasValue && categoryRepository != null)
        {
            var category = await categoryRepository.GetByIdAsync(task.CategoryId.Value);
            if (category == null)
            {
                throw new NotFoundException($"Category with id {task.CategoryId.Value} not found");
            }
        }

        // Validate ProjectId if provided
        if (task.ProjectId.HasValue && projectRepository != null)
        {
            var project = await projectRepository.GetByIdAsync(task.ProjectId.Value);
            if (project == null)
            {
                throw new NotFoundException($"Project with id {task.ProjectId.Value} not found");
            }
        }
    }
}
