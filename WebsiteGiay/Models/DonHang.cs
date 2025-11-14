using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteGiay.Models;

[Table("DonHang")]
public partial class DonHang
{
    [Key]
    public int MaDonHang { get; set; }

    public int MaNguoiDung { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayDatHang { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayGiaoHang { get; set; }

    [StringLength(255)]
    public string DiaChiGiaoHang { get; set; } = null!;

    [Column(TypeName = "decimal(18, 0)")]
    public decimal TongTien { get; set; }

    [StringLength(50)]
    public string TrangThai { get; set; } = null!;

    [StringLength(500)]
    public string? GhiChu { get; set; }

    [InverseProperty("MaDonHangNavigation")]
    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; } = new List<ChiTietDonHang>();

    [InverseProperty("MaDonHangNavigation")]
    public virtual ICollection<KhieuNai> KhieuNais { get; } = new List<KhieuNai>();

    [ForeignKey("MaNguoiDung")]
    [InverseProperty("DonHangs")]
    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;
}
