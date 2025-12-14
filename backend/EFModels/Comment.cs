using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend.EFModels;

[Index("CreatedAt", Name = "IX_Comments_CreatedAt")]
[Index("TaskId", Name = "IX_Comments_TaskId")]
[Index("TaskId", "CreatedAt", Name = "IX_Comments_TaskId_CreatedAt")]
[Index("UserId", Name = "IX_Comments_UserId")]
public partial class Comment
{
    [Key]
    public int Id { get; set; }

    public int TaskId { get; set; }

    public int UserId { get; set; }

    [StringLength(1000)]
    public string Content { get; set; } = null!;

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("TaskId")]
    [InverseProperty("Comments")]
    public virtual TaskModel Task { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Comments")]
    public virtual User User { get; set; } = null!;
}
