using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend.EFModels;

public partial class ProjectRole
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    public bool CanCreateTasks { get; set; }

    public bool CanEditTasks { get; set; }

    public bool CanDeleteTasks { get; set; }

    public bool CanAssignTasks { get; set; }

    public bool CanManageMembers { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
}
