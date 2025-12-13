namespace backend.Application.StoredProcedures;

/// <summary>
/// Output DTO from CreateTaskWithValidation stored procedure
/// </summary>
public class CreateTaskResult
{
    public bool Success { get; set; }
    public int TaskId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Message { get; set; } = string.Empty;
}

