using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteGiay.Models;

[Table("GioHang")]
public partial class GioHang
{
    [Key]
    public int MaGioHang { get; set; }

    public int MaNguoiDung { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [InverseProperty("MaGioHangNavigation")]
    public virtual ICollection<GioHangItem> GioHangItems { get; } = new List<GioHangItem>();

    [ForeignKey("MaNguoiDung")]
    [InverseProperty("GioHangs")]
    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;
}
