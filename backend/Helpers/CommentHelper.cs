using backend.Exceptions;
using backend.Interfaces;
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
        var task = await taskRepository.GetByIdAsync(comment.TaskId);
        if (task == null)
        {
            throw new NotFoundException($"Task with id {comment.TaskId} not found");
        }

        // Validate UserId exists
        var user = await userRepository.GetByIdAsync(comment.UserId);
        if (user == null)
        {
            throw new NotFoundException($"User with id {comment.UserId} not found");
        }
    }
}
