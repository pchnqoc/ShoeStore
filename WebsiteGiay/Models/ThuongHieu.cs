using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteGiay.Models;

[Table("ThuongHieu")]
public partial class ThuongHieu
{
    [Key]
    public int MaThuongHieu { get; set; }

    [StringLength(255)]
    public string TenThuongHieu { get; set; } = null!;

    [StringLength(100)]
    public string? QuocGia { get; set; }

    [StringLength(1000)]
    public string? MoTa { get; set; }

    [InverseProperty("MaThuongHieuNavigation")]
    public virtual ICollection<GiamGia> GiamGia { get; } = new List<GiamGia>();

    [InverseProperty("MaThuongHieuNavigation")]
    public virtual ICollection<SanPham> SanPhams { get; } = new List<SanPham>();
}
