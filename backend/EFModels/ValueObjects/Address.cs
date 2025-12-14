namespace backend.EFModels.ValueObjects;

/// <summary>
/// Address value object for owned entity pattern
/// Stored in the same table as the parent entity (Project)
/// </summary>
public class Address
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}


