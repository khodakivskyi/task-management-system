using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend.EFModels;

[Index("CreatedAt", Name = "IX_Favorites_CreatedAt")]
[Index("EntityTypeId", Name = "IX_Favorites_EntityTypeId")]
[Index("UserId", Name = "IX_Favorites_UserId")]
public partial class Favorite
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public int EntityId { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    public int EntityTypeId { get; set; }

    [ForeignKey("EntityTypeId")]
    [InverseProperty("Favorites")]
    public virtual EntityType EntityType { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Favorites")]
    public virtual User User { get; set; } = null!;
}
