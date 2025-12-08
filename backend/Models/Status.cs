namespace backend.Models;

/// <summary>
/// Represents a task status
/// </summary>
public class Status
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }

    public Status() { }
}

