namespace backend.Models;

/// <summary>
/// Represents a user's membership in a project with a specific role
/// </summary>
public class ProjectMember
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public DateTime JoinedAt { get; set; }

    public ProjectMember() { }
}

