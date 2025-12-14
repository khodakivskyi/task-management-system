namespace backend.EFModels;

/// <summary>
/// Partial class to extend User with soft delete properties
/// - IsDeleted and DeletedAt for soft delete (Global Query Filter)
/// </summary>
public partial class User
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
}


