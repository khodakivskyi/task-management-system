namespace backend.Models;

/// <summary>
/// Represents a task category
/// </summary>
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;

    public Category() { }
}

