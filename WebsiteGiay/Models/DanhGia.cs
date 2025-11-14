using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteGiay.Models;

[Index("MaNguoiDung", "MaSanPham", Name = "UQ__DanhGia__0A95A32109D8116C", IsUnique = true)]
public partial class DanhGia
{
    [Key]
    public int IdDanhGia { get; set; }

    public int MaSanPham { get; set; }

    public int MaNguoiDung { get; set; }

    public int? DiemSo { get; set; }

    public string? NhanXet { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayDanhGia { get; set; }

    public bool? TrangThai { get; set; }

    [ForeignKey("MaNguoiDung")]
    [InverseProperty("DanhGia")]
    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;

    [ForeignKey("MaSanPham")]
    [InverseProperty("DanhGia")]
    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}
