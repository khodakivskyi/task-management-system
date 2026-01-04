namespace backend.GraphQL.Comments.Inputs;

/// <summary>
/// Input type for updating an existing comment
/// </summary>
public class UpdateCommentInput
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
}
