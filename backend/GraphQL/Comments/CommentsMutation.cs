using backend.GraphQL.Comments.Inputs;
using backend.Models;
using backend.Services.Interfaces;

namespace backend.GraphQL.Mutations;

/// <summary>
/// GraphQL Mutation operations for Comments
/// </summary>
public class CommentsMutation
{
    public async Task<Comment> CreateComment(
        CreateCommentInput input,
        [Service] ICommentService commentService)
    {
        var comment = new Comment
        {
            TaskId = input.TaskId,
            UserId = input.UserId,
            Content = input.Content
        };

        return await commentService.CreateAsync(comment);
    }

    public async Task<Comment> UpdateComment(
        UpdateCommentInput input,
        [Service] ICommentService commentService)
    {
        var comment = new Comment
        {
            Id = input.Id,
            Content = input.Content
        };

        return await commentService.UpdateAsync(comment);
    }

    public async Task<bool> DeleteComment(
        int id,
        [Service] ICommentService commentService)
    {
        await commentService.DeleteAsync(id);
        return true;
    }
}
