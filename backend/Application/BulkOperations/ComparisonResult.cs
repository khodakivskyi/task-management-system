namespace backend.Application.BulkOperations;

/// <summary>
/// Comparison results for different bulk insert methods
/// </summary>
public class ComparisonResult
{
    public int RecordCount { get; set; }
    public List<BulkInsertResult> Results { get; set; } = new();
}

