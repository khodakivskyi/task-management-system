namespace backend.CodeFirst.Entities;

/// <summary>
/// Code-First entity: Status
/// Clean entity class without scaffolding annotations
/// Configuration is done through Fluent API in StatusConfiguration
/// </summary>
public class Status
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Color { get; set; }

    // Navigation properties
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}

