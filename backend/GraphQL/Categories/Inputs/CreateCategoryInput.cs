namespace backend.GraphQL.Categories.Inputs;

/// <summary>
/// Input type for creating a new category
/// </summary>
public class CreateCategoryInput
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}
