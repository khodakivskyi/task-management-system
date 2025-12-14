using backend.EFModels.Enums;
using backend.EFModels.ValueObjects;

namespace backend.EFModels;

/// <summary>
/// Partial class to extend TaskModel with advanced pattern properties
/// - IsDeleted and DeletedAt for soft delete (Global Query Filter)
/// - PriorityLevel enum for value converter demonstration
/// - Metadata for JSON value converter demonstration
/// </summary>
public partial class TaskModel
{
    /// <summary>
    /// Soft delete flag - used with Global Query Filter
    /// When true, entity is considered deleted and filtered out by default
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Timestamp when entity was soft deleted
    /// Null if entity is not deleted
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Task priority level enum
    /// Stored as string in database using value converter
    /// </summary>
    public TaskPriorityLevel? PriorityLevel { get; set; }

    /// <summary>
    /// Task metadata complex object
    /// Stored as JSON string in database using value converter
    /// </summary>
    public TaskMetadata? Metadata { get; set; }
}


