using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteGiay.Models;

[Table("BaoCaoBanHang")]
public partial class BaoCaoBanHang
{
    [Key]
    public int MaBaoCao { get; set; }

    [Column(TypeName = "date")]
    public DateTime NgayBaoCao { get; set; }

    public int? TongDonHang { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? TongDoanhThu { get; set; }

    [StringLength(255)]
    public string? SanPhamBanChay { get; set; }
}
