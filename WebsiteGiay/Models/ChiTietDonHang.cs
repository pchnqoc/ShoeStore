using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteGiay.Models;

[PrimaryKey("MaDonHang", "MaBienTheSp")]
[Table("ChiTietDonHang")]
public partial class ChiTietDonHang
{
    [Key]
    public int MaDonHang { get; set; }

    [Key]
    [Column("MaBienTheSP")]
    public int MaBienTheSp { get; set; }

    public int SoLuong { get; set; }

    [Column(TypeName = "decimal(12, 0)")]
    public decimal DonGia { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal ThanhTien { get; set; }

    [InverseProperty("Ma")]
    public virtual ICollection<KhieuNai> KhieuNais { get; } = new List<KhieuNai>();

    [ForeignKey("MaBienTheSp")]
    [InverseProperty("ChiTietDonHangs")]
    public virtual BienTheSanPham MaBienTheSpNavigation { get; set; } = null!;

    [ForeignKey("MaDonHang")]
    [InverseProperty("ChiTietDonHangs")]
    public virtual DonHang MaDonHangNavigation { get; set; } = null!;
}
