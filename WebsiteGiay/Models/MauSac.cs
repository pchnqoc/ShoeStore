using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteGiay.Models;

[Table("MauSac")]
[Index("TenMau", Name = "UQ__MauSac__332F6A91ADC398E2", IsUnique = true)]
public partial class MauSac
{
    [Key]
    public int MaMau { get; set; }

    [StringLength(20)]
    public string TenMau { get; set; } = null!;

    [InverseProperty("MaMauNavigation")]
    public virtual ICollection<BienTheSanPham> BienTheSanPhams { get; } = new List<BienTheSanPham>();
}
