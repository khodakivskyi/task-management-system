namespace backend.Models;

/// <summary>
/// Represents a role within a project with specific permissions
/// </summary>
public class ProjectRole
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool CanCreateTasks { get; set; }
    public bool CanEditTasks { get; set; }
    public bool CanDeleteTasks { get; set; }
    public bool CanAssignTasks { get; set; }
    public bool CanManageMembers { get; set; }

    public ProjectRole() { }
}

