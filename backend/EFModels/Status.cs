using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend.EFModels;

public partial class Status
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [StringLength(7)]
    public string? Color { get; set; }

    [InverseProperty("Status")]
    public virtual ICollection<TaskModel> Tasks { get; set; } = new List<TaskModel>();
}
