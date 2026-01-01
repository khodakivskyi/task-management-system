using backend.Models;

namespace backend.Services.Interfaces;

/// <summary>
/// Service interface for Comment operations
/// </summary>
public interface ICommentService
{
    Task<Comment> CreateAsync(Comment comment);
    Task<Comment?> GetByIdAsync(int id);
    Task<IEnumerable<Comment>> GetAllAsync();
    Task<Comment> UpdateAsync(Comment comment);
    Task DeleteAsync(int id);
}
