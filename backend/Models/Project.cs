namespace backend.Models;

/// <summary>
/// Represents a project
/// </summary>
public class Project
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public Project() { }
}

