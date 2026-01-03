namespace backend.GraphQL.Categories.Inputs;

/// <summary>
/// Input type for updating an existing category
/// </summary>
public class UpdateCategoryInput
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}
