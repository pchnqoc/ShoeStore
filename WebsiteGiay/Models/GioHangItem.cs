using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteGiay.Models;

[PrimaryKey("MaGioHang", "MaBienTheSp")]
[Table("GioHangItem")]
public partial class GioHangItem
{
    [Key]
    public int MaGioHang { get; set; }

    [Key]
    [Column("MaBienTheSP")]
    public int MaBienTheSp { get; set; }

    public int SoLuong { get; set; }

    [Column(TypeName = "decimal(12, 0)")]
    public decimal DonGia { get; set; }

    [ForeignKey("MaBienTheSp")]
    [InverseProperty("GioHangItems")]
    public virtual BienTheSanPham MaBienTheSpNavigation { get; set; } = null!;

    [ForeignKey("MaGioHang")]
    [InverseProperty("GioHangItems")]
    public virtual GioHang MaGioHangNavigation { get; set; } = null!;
}
