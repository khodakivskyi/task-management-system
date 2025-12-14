using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend.EFModels;

[Index("TaskId", Name = "IX_TaskAssignees_TaskId")]
[Index("TaskId", "UserId", Name = "IX_TaskAssignees_TaskId_UserId", IsUnique = true)]
[Index("UserId", Name = "IX_TaskAssignees_UserId")]
[Index("UserId", "TaskId", Name = "IX_TaskAssignees_UserId_TaskId")]
public partial class TaskAssignee
{
    [Key]
    public int Id { get; set; }

    public int TaskId { get; set; }

    public int UserId { get; set; }

    [ForeignKey("TaskId")]
    [InverseProperty("TaskAssignees")]
    public virtual TaskModel Task { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("TaskAssignees")]
    public virtual User User { get; set; } = null!;
}
