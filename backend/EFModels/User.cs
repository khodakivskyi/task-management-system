using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend.EFModels;

[Index("Login", Name = "IX_Users_Login", IsUnique = true)]
[Index("Name", Name = "IX_Users_Name")]
[Index("Name", "Surname", Name = "IX_Users_Name_Surname")]
[Index("Surname", Name = "IX_Users_Surname")]
public partial class User
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string Name { get; set; } = null!;

    [StringLength(255)]
    public string? Surname { get; set; }

    [StringLength(255)]
    public string Login { get; set; } = null!;

    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(255)]
    public string Salt { get; set; } = null!;

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    [InverseProperty("User")]
    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    [InverseProperty("User")]
    public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();

    [InverseProperty("Owner")]
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    [InverseProperty("User")]
    public virtual ICollection<TaskAssignee> TaskAssignees { get; set; } = new List<TaskAssignee>();

    [InverseProperty("User")]
    public virtual ICollection<TaskHistory> TaskHistories { get; set; } = new List<TaskHistory>();

    [InverseProperty("Owner")]
    public virtual ICollection<TaskModel> Tasks { get; set; } = new List<TaskModel>();
}
