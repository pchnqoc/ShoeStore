using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteGiay.Models;

public partial class GiamGia
{
    [Key]
    public int IdGiamGia { get; set; }

    public int? MaSanPham { get; set; }

    public int? MaDanhMuc { get; set; }

    public int? MaThuongHieu { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? PhanTramGiam { get; set; }

    [Column(TypeName = "decimal(12, 0)")]
    public decimal? GiaTriGiam { get; set; }

    [Column(TypeName = "date")]
    public DateTime NgayBatDau { get; set; }

    [Column(TypeName = "date")]
    public DateTime NgayKetThuc { get; set; }

    [StringLength(500)]
    public string? MoTa { get; set; }

    [ForeignKey("MaDanhMuc")]
    [InverseProperty("GiamGia")]
    public virtual DanhMucGiay? MaDanhMucNavigation { get; set; }

    [ForeignKey("MaSanPham")]
    [InverseProperty("GiamGia")]
    public virtual SanPham? MaSanPhamNavigation { get; set; }

    [ForeignKey("MaThuongHieu")]
    [InverseProperty("GiamGia")]
    public virtual ThuongHieu? MaThuongHieuNavigation { get; set; }
}
