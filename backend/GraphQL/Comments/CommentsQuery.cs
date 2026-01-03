using backend.Models;
using backend.Services.Interfaces;

namespace backend.GraphQL.Queries;

/// <summary>
/// GraphQL Query operations for Comments
/// </summary>
public class CommentsQuery
{
    public async Task<IEnumerable<Comment>> GetComments(
        [Service] ICommentService commentService)
    {
        return await commentService.GetAllAsync();
    }

    public async Task<Comment?> GetCommentById(
        int id,
        [Service] ICommentService commentService)
    {
        return await commentService.GetByIdAsync(id);
    }
}
