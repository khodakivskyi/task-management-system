namespace backend.CodeFirst.Entities;

/// <summary>
/// Code-First entity: User
/// Clean entity class without scaffolding annotations
/// Configuration is done through Fluent API in UserConfiguration
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Surname { get; set; }
    public string Login { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Salt { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    // Navigation properties - only User, Task, Project relationships
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}

