using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteGiay.Models;

[Table("DanhMucGiay")]
public partial class DanhMucGiay
{
    [Key]
    public int MaDanhMuc { get; set; }

    [StringLength(255)]
    public string TenDanhMuc { get; set; } = null!;

    [StringLength(1000)]
    public string? MoTa { get; set; }

    [InverseProperty("MaDanhMucNavigation")]
    public virtual ICollection<GiamGia> GiamGia { get; } = new List<GiamGia>();

    [InverseProperty("MaDanhMucNavigation")]
    public virtual ICollection<SanPham> SanPhams { get; } = new List<SanPham>();
}
