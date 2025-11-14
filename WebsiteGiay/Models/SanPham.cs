using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteGiay.Models;

[Table("SanPham")]
public partial class SanPham
{
    [Key]
    public int MaSanPham { get; set; }

    [StringLength(255)]
    public string TenSanPham { get; set; } = null!;

    [StringLength(1000)]
    public string? MoTa { get; set; }

    [Column(TypeName = "decimal(12, 0)")]
    public decimal Gia { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string? HinhAnh { get; set; }

    public int MaDanhMuc { get; set; }

    public int MaThuongHieu { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [InverseProperty("MaSanPhamNavigation")]
    public virtual ICollection<BienTheSanPham> BienTheSanPhams { get; } = new List<BienTheSanPham>();

    [InverseProperty("MaSanPhamNavigation")]
    public virtual ICollection<DanhGia> DanhGia { get; } = new List<DanhGia>();

    [InverseProperty("MaSanPhamNavigation")]
    public virtual ICollection<GiamGia> GiamGia { get; } = new List<GiamGia>();

    [ForeignKey("MaDanhMuc")]
    [InverseProperty("SanPhams")]
    public virtual DanhMucGiay MaDanhMucNavigation { get; set; } = null!;

    [ForeignKey("MaThuongHieu")]
    [InverseProperty("SanPhams")]
    public virtual ThuongHieu MaThuongHieuNavigation { get; set; } = null!;
}
