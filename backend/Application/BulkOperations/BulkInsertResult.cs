namespace backend.Application.BulkOperations;

/// <summary>
/// Result of bulk insert operation with performance metrics
/// </summary>
public class BulkInsertResult
{
    public string Method { get; set; } = string.Empty;
    public int TotalRecords { get; set; }
    public int InsertedRecords { get; set; }
    public long ElapsedMilliseconds { get; set; }
    public double RecordsPerSecond { get; set; }
}

