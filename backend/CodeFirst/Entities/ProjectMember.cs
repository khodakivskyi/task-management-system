namespace backend.CodeFirst.Entities;

/// <summary>
/// Code-First entity: ProjectMember
/// Clean entity class without scaffolding annotations
/// Configuration is done through Fluent API in ProjectMemberConfiguration
/// Represents many-to-many relationship between Project and User with additional Role
/// </summary>
public class ProjectMember
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public DateTime JoinedAt { get; set; }

    // Navigation properties
    public Project Project { get; set; } = null!;
    public User User { get; set; } = null!;
    public ProjectRole Role { get; set; } = null!;
}

