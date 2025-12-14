namespace backend.CodeFirst.Entities;

/// <summary>
/// Code-First entity: Category
/// Clean entity class without scaffolding annotations
/// Configuration is done through Fluent API in CategoryConfiguration
/// </summary>
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Color { get; set; } = null!;

    // Navigation properties
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}

