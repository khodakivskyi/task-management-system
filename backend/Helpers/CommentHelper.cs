using backend.Exceptions;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;

namespace backend.Helpers;

/// <summary>
/// Validation helper for Comment entity
/// </summary>
public static class CommentHelper
{
    /// <summary>
    /// Validates a comment entity
    /// </summary>
    public static async Task ValidateCommentAsync(
        Comment comment,
        IRepository<TaskModel> taskRepository,
        IRepository<User> userRepository)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(comment.Content))
        {
            throw new ValidationException("Comment content is required");
        }

        // Validate TaskId exists
        await EntityValidationHelper.EnsureEntityExistsAsync(comment.TaskId, taskRepository, "Task");

        // Validate UserId exists
        await EntityValidationHelper.EnsureEntityExistsAsync(comment.UserId, userRepository, "User");
    }
}
