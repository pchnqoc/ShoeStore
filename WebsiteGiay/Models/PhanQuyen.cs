using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteGiay.Models;

[PrimaryKey("MaVaiTro", "TenChucNang")]
[Table("PhanQuyen")]
public partial class PhanQuyen
{
    [Key]
    public int MaVaiTro { get; set; }

    [Key]
    [StringLength(100)]
    public string TenChucNang { get; set; } = null!;

    public bool? DuocTruyCap { get; set; }

    [ForeignKey("MaVaiTro")]
    [InverseProperty("PhanQuyens")]
    public virtual VaiTro MaVaiTroNavigation { get; set; } = null!;
}
