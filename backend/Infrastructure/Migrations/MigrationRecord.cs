namespace backend.Infrastructure.Migrations;

/// <summary>
/// Represents a migration file and its metadata
/// </summary>
internal class MigrationRecord
{
    public string Version { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string SqlContent { get; set; } = string.Empty;
    public string Checksum { get; set; } = string.Empty;

    // Applied migration info
    public bool IsApplied { get; set; }
    public DateTime? AppliedAt { get; set; }
    public int? ExecutionTimeMs { get; set; }
    public string? DatabaseChecksum { get; set; }

    /// Checks if the file checksum matches the database checksum
    public bool ChecksumMatches =>
        IsApplied && DatabaseChecksum != null && DatabaseChecksum.Equals(Checksum, StringComparison.OrdinalIgnoreCase);
}
