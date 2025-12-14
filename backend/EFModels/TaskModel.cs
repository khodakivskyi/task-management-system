using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend.EFModels;

[Index("CategoryId", Name = "IX_Tasks_CategoryId")]
[Index("CreatedAt", Name = "IX_Tasks_CreatedAt")]
[Index("Deadline", Name = "IX_Tasks_Deadline")]
[Index("OwnerId", Name = "IX_Tasks_OwnerId")]
[Index("OwnerId", "StatusId", Name = "IX_Tasks_OwnerId_StatusId")]
[Index("Priority", Name = "IX_Tasks_Priority")]
[Index("ProjectId", Name = "IX_Tasks_ProjectId")]
[Index("ProjectId", "StatusId", Name = "IX_Tasks_ProjectId_StatusId")]
[Index("StatusId", Name = "IX_Tasks_StatusId")]
[Index("UpdatedAt", Name = "IX_Tasks_UpdatedAt")]
public partial class TaskModel
{
    [Key]
    public int Id { get; set; }

    public int OwnerId { get; set; }

    public int StatusId { get; set; }

    public int? CategoryId { get; set; }

    public int? ProjectId { get; set; }

    [StringLength(50)]
    public string Title { get; set; } = null!;

    [StringLength(250)]
    public string? Description { get; set; }

    public int? Priority { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? Deadline { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime UpdatedAt { get; set; }

    public int EstimatedHours { get; set; }

    public int ActualHours { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Tasks")]
    public virtual Category? Category { get; set; }

    [InverseProperty("Task")]
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    [ForeignKey("OwnerId")]
    [InverseProperty("Tasks")]
    public virtual User Owner { get; set; } = null!;

    [ForeignKey("ProjectId")]
    [InverseProperty("Tasks")]
    public virtual Project? Project { get; set; }

    [ForeignKey("StatusId")]
    [InverseProperty("Tasks")]
    public virtual Status Status { get; set; } = null!;

    [InverseProperty("Task")]
    public virtual ICollection<TaskAssignee> TaskAssignees { get; set; } = new List<TaskAssignee>();

    [InverseProperty("Task")]
    public virtual ICollection<TaskHistory> TaskHistories { get; set; } = new List<TaskHistory>();
}
