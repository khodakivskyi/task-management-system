using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend.EFModels;

[Table("TaskHistory")]
[Index("ChangedAt", Name = "IX_TaskHistory_ChangedAt")]
[Index("FieldName", Name = "IX_TaskHistory_FieldName")]
[Index("TaskId", Name = "IX_TaskHistory_TaskId")]
[Index("TaskId", "ChangedAt", Name = "IX_TaskHistory_TaskId_ChangedAt")]
[Index("TaskId", "UserId", Name = "IX_TaskHistory_TaskId_UserId")]
[Index("UserId", Name = "IX_TaskHistory_UserId")]
public partial class TaskHistory
{
    [Key]
    public int Id { get; set; }

    public int TaskId { get; set; }

    public int UserId { get; set; }

    [StringLength(100)]
    public string FieldName { get; set; } = null!;

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime ChangedAt { get; set; }

    [ForeignKey("TaskId")]
    [InverseProperty("TaskHistories")]
    public virtual TaskModel Task { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("TaskHistories")]
    public virtual User User { get; set; } = null!;
}
