using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend.EFModels;

[Table("__MigrationsHistory")]
public partial class MigrationsHistory
{
    [Key]
    [StringLength(10)]
    public string MigrationVersion { get; set; } = null!;

    [StringLength(255)]
    public string FileName { get; set; } = null!;

    [StringLength(64)]
    public string Checksum { get; set; } = null!;

    [Column(TypeName = "timestamp without time zone")]
    public DateTime AppliedAt { get; set; }

    public int ExecutionTimeMs { get; set; }
}
