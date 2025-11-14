using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteGiay.Models;

[Table("BienTheSanPham")]
[Index("MaSanPham", "MaKichCo", "MaMau", Name = "UQ_BienThe_SanPham_KichCo_Mau", IsUnique = true)]
public partial class BienTheSanPham
{
    [Key]
    [Column("MaBienTheSP")]
    public int MaBienTheSp { get; set; }

    public int MaSanPham { get; set; }

    public int MaKichCo { get; set; }

    public int MaMau { get; set; }

    public int SoLuongTon { get; set; }

    [InverseProperty("MaBienTheSpNavigation")]
    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; } = new List<ChiTietDonHang>();

    [InverseProperty("MaBienTheSpNavigation")]
    public virtual ICollection<GioHangItem> GioHangItems { get; } = new List<GioHangItem>();

    [ForeignKey("MaKichCo")]
    [InverseProperty("BienTheSanPhams")]
    public virtual KichCo MaKichCoNavigation { get; set; } = null!;

    [ForeignKey("MaMau")]
    [InverseProperty("BienTheSanPhams")]
    public virtual MauSac MaMauNavigation { get; set; } = null!;

    [ForeignKey("MaSanPham")]
    [InverseProperty("BienTheSanPhams")]
    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}
