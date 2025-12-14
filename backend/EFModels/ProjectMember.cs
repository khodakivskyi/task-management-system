using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend.EFModels;

[Index("ProjectId", Name = "IX_ProjectMembers_ProjectId")]
[Index("ProjectId", "RoleId", Name = "IX_ProjectMembers_ProjectId_RoleId")]
[Index("ProjectId", "UserId", Name = "IX_ProjectMembers_ProjectId_UserId", IsUnique = true)]
[Index("RoleId", Name = "IX_ProjectMembers_RoleId")]
[Index("UserId", Name = "IX_ProjectMembers_UserId")]
[Index("UserId", "ProjectId", Name = "IX_ProjectMembers_UserId_ProjectId")]
public partial class ProjectMember
{
    [Key]
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public int UserId { get; set; }

    public int RoleId { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime JoinedAt { get; set; }

    [ForeignKey("ProjectId")]
    [InverseProperty("ProjectMembers")]
    public virtual Project Project { get; set; } = null!;

    [ForeignKey("RoleId")]
    [InverseProperty("ProjectMembers")]
    public virtual ProjectRole Role { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("ProjectMembers")]
    public virtual User User { get; set; } = null!;
}
