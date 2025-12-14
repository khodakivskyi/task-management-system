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

    // Navigation properties
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<TaskAssignee> TaskAssignees { get; set; } = new List<TaskAssignee>();
    public ICollection<TaskHistory> TaskHistories { get; set; } = new List<TaskHistory>();
    public ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}

