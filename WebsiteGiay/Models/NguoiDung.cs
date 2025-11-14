using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteGiay.Models;

[Table("NguoiDung")]
[Index("TenDangNhap", Name = "UQ__NguoiDun__55F68FC015A20C4C", IsUnique = true)]
[Index("Email", Name = "UQ__NguoiDun__A9D105349CC1CB97", IsUnique = true)]
public partial class NguoiDung
{
    [Key]
    public int MaNguoiDung { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string TenDangNhap { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string MatKhau { get; set; } = null!;

    [StringLength(100)]
    public string HoTen { get; set; } = null!;

    [Column(TypeName = "date")]
    public DateTime? NgaySinh { get; set; }

    [StringLength(8)]
    public string? GioiTinh { get; set; }

    [StringLength(15)]
    [Unicode(false)]
    public string? DienThoai { get; set; }

    [StringLength(255)]
    public string? DiaChi { get; set; }

    public int MaVaiTro { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    public int? SoLanMua { get; set; }

    [InverseProperty("MaNguoiDungNavigation")]
    public virtual ICollection<DanhGia> DanhGia { get; } = new List<DanhGia>();

    [InverseProperty("MaNguoiDungNavigation")]
    public virtual ICollection<DonHang> DonHangs { get; } = new List<DonHang>();

    [InverseProperty("MaNguoiDungNavigation")]
    public virtual ICollection<GioHang> GioHangs { get; } = new List<GioHang>();

    [InverseProperty("MaNguoiDungNavigation")]
    public virtual ICollection<KhieuNai> KhieuNaiMaNguoiDungNavigations { get; } = new List<KhieuNai>();

    [InverseProperty("NguoiXuLyNavigation")]
    public virtual ICollection<KhieuNai> KhieuNaiNguoiXuLyNavigations { get; } = new List<KhieuNai>();

    [ForeignKey("MaVaiTro")]
    [InverseProperty("NguoiDungs")]
    public virtual VaiTro MaVaiTroNavigation { get; set; } = null!;

    [InverseProperty("MaNguoiTraLoiNavigation")]
    public virtual ICollection<PhanHoiKhieuNai> PhanHoiKhieuNais { get; } = new List<PhanHoiKhieuNai>();
}
