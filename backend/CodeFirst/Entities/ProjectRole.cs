namespace backend.CodeFirst.Entities;

/// <summary>
/// Code-First entity: ProjectRole
/// Clean entity class without scaffolding annotations
/// Configuration is done through Fluent API in ProjectRoleConfiguration
/// </summary>
public class ProjectRole
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool CanCreateTasks { get; set; }
    public bool CanEditTasks { get; set; }
    public bool CanDeleteTasks { get; set; }
    public bool CanAssignTasks { get; set; }
    public bool CanManageMembers { get; set; }

    // Navigation properties
    public ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
}

