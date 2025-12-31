using backend.Exceptions;
using backend.Interfaces;
using backend.Models;

namespace backend.Services;

/// <summary>
/// Service for Comment operations with business logic and validation
/// </summary>
public class CommentService : ICommentService
{
    private readonly IRepository<Comment> _commentRepository;
    private readonly IRepository<TaskModel> _taskRepository;
    private readonly IRepository<User> _userRepository;

    public CommentService(
        IRepository<Comment> commentRepository,
        IRepository<TaskModel> taskRepository,
        IRepository<User> userRepository)
    {
        _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<Comment> CreateAsync(Comment comment)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(comment.Content))
        {
            throw new ValidationException("Comment content is required");
        }

        // Validate TaskId exists
        var task = await _taskRepository.GetByIdAsync(comment.TaskId);
        if (task == null)
        {
            throw new NotFoundException($"Task with id {comment.TaskId} not found");
        }

        // Validate UserId exists
        var user = await _userRepository.GetByIdAsync(comment.UserId);
        if (user == null)
        {
            throw new NotFoundException($"User with id {comment.UserId} not found");
        }

        // Set timestamp if not provided
        if (comment.CreatedAt == default)
        {
            comment.CreatedAt = DateTime.UtcNow;
        }

        var id = await _commentRepository.CreateAsync(comment);
        comment.Id = id;
        return comment;
    }

    public async Task<Comment?> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new BadRequestException("Comment id must be greater than 0");
        }

        return await _commentRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Comment>> GetAllAsync()
    {
        return await _commentRepository.GetAllAsync();
    }

    public async Task<Comment> UpdateAsync(Comment comment)
    {
        if (comment.Id <= 0)
        {
            throw new BadRequestException("Comment id must be greater than 0");
        }

        // Check if comment exists
        var existingComment = await _commentRepository.GetByIdAsync(comment.Id);
        if (existingComment == null)
        {
            throw new NotFoundException($"Comment with id {comment.Id} not found");
        }

        // Validation
        if (string.IsNullOrWhiteSpace(comment.Content))
        {
            throw new ValidationException("Comment content is required");
        }

        // Preserve TaskId, UserId, and CreatedAt
        comment.TaskId = existingComment.TaskId;
        comment.UserId = existingComment.UserId;
        comment.CreatedAt = existingComment.CreatedAt;

        var updated = await _commentRepository.UpdateAsync(comment);
        if (!updated)
        {
            throw new NotFoundException($"Failed to update comment with id {comment.Id}");
        }

        return comment;
    }

    public async Task DeleteAsync(int id)
    {
        if (id <= 0)
        {
            throw new BadRequestException("Comment id must be greater than 0");
        }

        var comment = await _commentRepository.GetByIdAsync(id);
        if (comment == null)
        {
            throw new NotFoundException($"Comment with id {id} not found");
        }

        var deleted = await _commentRepository.DeleteAsync(id);
        if (!deleted)
        {
            throw new NotFoundException($"Failed to delete comment with id {id}");
        }
    }
}
