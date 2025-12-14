namespace backend.CodeFirst.Entities;

/// <summary>
/// Code-First entity: Project
/// Clean entity class without scaffolding annotations
/// Configuration is done through Fluent API in ProjectConfiguration
/// </summary>
public class Project
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // Navigation properties
    public User Owner { get; set; } = null!;
    public ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}

