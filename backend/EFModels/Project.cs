using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend.EFModels;

[Index("EndDate", Name = "IX_Projects_EndDate")]
[Index("Name", Name = "IX_Projects_Name")]
[Index("OwnerId", Name = "IX_Projects_OwnerId")]
[Index("StartDate", Name = "IX_Projects_StartDate")]
public partial class Project
{
    [Key]
    public int Id { get; set; }

    public int OwnerId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime EndDate { get; set; }

    [ForeignKey("OwnerId")]
    [InverseProperty("Projects")]
    public virtual User Owner { get; set; } = null!;

    [InverseProperty("Project")]
    public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();

    [InverseProperty("Project")]
    public virtual ICollection<TaskModel> Tasks { get; set; } = new List<TaskModel>();
}
