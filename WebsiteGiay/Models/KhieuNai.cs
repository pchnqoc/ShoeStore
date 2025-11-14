using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteGiay.Models;

[Table("KhieuNai")]
public partial class KhieuNai
{
    [Key]
    public int MaKhieuNai { get; set; }

    public int MaNguoiDung { get; set; }

    public int? MaDonHang { get; set; }

    [Column("MaBienTheSP")]
    public int? MaBienTheSp { get; set; }

    [StringLength(255)]
    public string? TieuDe { get; set; }

    public string? NoiDung { get; set; }

    [StringLength(50)]
    public string? TrangThai { get; set; }

    public int? NguoiXuLy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [ForeignKey("MaDonHang, MaBienTheSp")]
    [InverseProperty("KhieuNais")]
    public virtual ChiTietDonHang? Ma { get; set; }

    [ForeignKey("MaDonHang")]
    [InverseProperty("KhieuNais")]
    public virtual DonHang? MaDonHangNavigation { get; set; }

    [ForeignKey("MaNguoiDung")]
    [InverseProperty("KhieuNaiMaNguoiDungNavigations")]
    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;

    [ForeignKey("NguoiXuLy")]
    [InverseProperty("KhieuNaiNguoiXuLyNavigations")]
    public virtual NguoiDung? NguoiXuLyNavigation { get; set; }

    [InverseProperty("MaKhieuNaiNavigation")]
    public virtual ICollection<PhanHoiKhieuNai> PhanHoiKhieuNais { get; } = new List<PhanHoiKhieuNai>();
}
