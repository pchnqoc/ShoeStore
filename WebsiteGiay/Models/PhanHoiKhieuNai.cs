using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteGiay.Models;

[Table("PhanHoiKhieuNai")]
public partial class PhanHoiKhieuNai
{
    [Key]
    public int IdPhanHoi { get; set; }

    public int MaKhieuNai { get; set; }

    public int MaNguoiTraLoi { get; set; }

    public string? NoiDung { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayPhanHoi { get; set; }

    [ForeignKey("MaKhieuNai")]
    [InverseProperty("PhanHoiKhieuNais")]
    public virtual KhieuNai MaKhieuNaiNavigation { get; set; } = null!;

    [ForeignKey("MaNguoiTraLoi")]
    [InverseProperty("PhanHoiKhieuNais")]
    public virtual NguoiDung MaNguoiTraLoiNavigation { get; set; } = null!;
}
