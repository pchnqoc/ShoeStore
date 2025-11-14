using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteGiay.Models;

[Table("VaiTro")]
[Index("TenVaiTro", Name = "UQ__VaiTro__1DA5581460326FE8", IsUnique = true)]
public partial class VaiTro
{
    [Key]
    public int MaVaiTro { get; set; }

    [StringLength(50)]
    public string TenVaiTro { get; set; } = null!;

    [InverseProperty("MaVaiTroNavigation")]
    public virtual ICollection<NguoiDung> NguoiDungs { get; } = new List<NguoiDung>();

    [InverseProperty("MaVaiTroNavigation")]
    public virtual ICollection<PhanQuyen> PhanQuyens { get; } = new List<PhanQuyen>();
}
