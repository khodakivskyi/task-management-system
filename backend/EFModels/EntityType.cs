using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend.EFModels;

[Index("Name", Name = "EntityTypes_Name_key", IsUnique = true)]
public partial class EntityType
{
    [Key]
    public int Id { get; set; }

    [StringLength(20)]
    public string Name { get; set; } = null!;

    [InverseProperty("EntityType")]
    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}
