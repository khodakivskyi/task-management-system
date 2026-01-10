    using backend.Exceptions;
using backend.Helpers;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using backend.Services.Interfaces;

namespace backend.Services;

/// <summary>
/// Service for Comment operations with business logic and validation
/// </summary>
public class CommentService : ICommentService
{
    private readonly IRepository<Comment> _commentRepository;
    private readonly IRepository<TaskModel> _taskRepository;
    private readonly IUserRepository _userRepository;

    private const string CommentEntity = nameof(Comment);

    public CommentService(
        IRepository<Comment> commentRepository,
        IRepository<TaskModel> taskRepository,
        IUserRepository userRepository)
    {
        _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<Comment> CreateAsync(Comment comment)
    {
        await CommentHelper.ValidateCommentAsync(comment, _taskRepository, _userRepository);

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
        ValidationHelper.ValidateId(id, CommentEntity);
        return await _commentRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Comment>> GetAllAsync()
    {
        return await _commentRepository.GetAllAsync();
    }

    public async Task<Comment> UpdateAsync(Comment comment)
    {
        ValidationHelper.ValidateId(comment.Id, CommentEntity);

        var existingComment = await EntityValidationHelper.EnsureEntityExistsAsync(comment.Id, _commentRepository, CommentEntity);

        await CommentHelper.ValidateCommentAsync(comment, _taskRepository, _userRepository);

        // Preserve TaskId, UserId, and CreatedAt
        comment.TaskId = existingComment.TaskId;
        comment.UserId = existingComment.UserId;
        comment.CreatedAt = existingComment.CreatedAt;

        var updated = await _commentRepository.UpdateAsync(comment);
        if (!updated)
        {
            throw new NotFoundException("Failed to update comment");
        }

        return comment;
    }

    public async Task DeleteAsync(int id)
    {
        ValidationHelper.ValidateId(id, CommentEntity);

        await EntityValidationHelper.EnsureEntityExistsAsync(id, _commentRepository, CommentEntity);

        var deleted = await _commentRepository.DeleteAsync(id);
        if (!deleted)
        {
            throw new NotFoundException("Failed to delete comment");
        }
    }
}
