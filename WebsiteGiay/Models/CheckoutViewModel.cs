using System.ComponentModel.DataAnnotations;

namespace WebsiteGiay.Models;

public class CheckoutViewModel
{
    public int MaNguoiDung { get; set; }
    
    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [Display(Name = "Họ và tên")]
    public string HoTen { get; set; } = null!;
    
    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [Display(Name = "Số điện thoại")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string DienThoai { get; set; } = null!;
    
    [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
    [Display(Name = "Địa chỉ giao hàng")]
    public string DiaChiGiaoHang { get; set; } = null!;
    
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }
    
    [Display(Name = "Phương thức thanh toán")]
    public string PhuongThucThanhToan { get; set; } = "COD"; // COD = Cash on Delivery
    
    public List<CheckoutItemViewModel> Items { get; set; } = new();
    
    public decimal TongTien { get; set; }
    
    public decimal PhiVanChuyen { get; set; } = 0;
    
    public decimal TongCong { get; set; }
}

public class CheckoutItemViewModel
{
    public int MaBienTheSp { get; set; }
    public int MaSanPham { get; set; }
    public string TenSanPham { get; set; } = null!;
    public string ThuongHieu { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public string KichCo { get; set; } = null!;
    public string MauSac { get; set; } = null!;
    public int SoLuong { get; set; }
    public decimal DonGia { get; set; }
    public decimal ThanhTien { get; set; }
}

