namespace backend.GraphQL.Comments.Inputs;

/// <summary>
/// Input type for creating a new comment
/// </summary>
public class CreateCommentInput
{
    public int TaskId { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; } = string.Empty;
}
