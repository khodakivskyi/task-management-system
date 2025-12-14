using backend.EFModels.ValueObjects;

namespace backend.EFModels;

/// <summary>
/// Partial class to extend Project with advanced pattern properties
/// - IsDeleted and DeletedAt for soft delete (Global Query Filter)
/// - Address value object for owned entity demonstration
/// </summary>
public partial class Project
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
    /// Project address value object
    /// Stored in the same table as Project using owned entity pattern
    /// Columns: Address_Street, Address_City, Address_ZipCode, Address_Country
    /// </summary>
    public Address? Address { get; set; }
}


