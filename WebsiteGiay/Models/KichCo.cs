using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteGiay.Models;

[Table("KichCo")]
[Index("KichCo1", Name = "UQ__KichCo__E377A0440ABCA9D0", IsUnique = true)]
public partial class KichCo
{
    [Key]
    public int MaKichCo { get; set; }

    [Column("KichCo")]
    [StringLength(10)]
    [Unicode(false)]
    public string KichCo1 { get; set; } = null!;

    [InverseProperty("MaKichCoNavigation")]
    public virtual ICollection<BienTheSanPham> BienTheSanPhams { get; } = new List<BienTheSanPham>();
}
