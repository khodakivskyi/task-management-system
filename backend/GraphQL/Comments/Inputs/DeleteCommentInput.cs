namespace backend.GraphQL.Comments.Inputs;

/// <summary>
/// Input type for deleting a comment
/// </summary>
public class DeleteCommentInput
{
    public int Id { get; set; }
    public int UserId { get; set; }
}
