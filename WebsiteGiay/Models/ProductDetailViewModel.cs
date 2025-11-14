using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebsiteGiay.Models
{
    public class ProductDetailViewModel
    {
        public int MaSanPham { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public decimal Gia { get; set; }
        public string? HinhAnh { get; set; }
        public List<SelectListItem> DanhSachMau { get; set; } = new();
        public List<SelectListItem> DanhSachKichCo { get; set; } = new();
        public int SoLuongTon { get; set; }
        public List<ProductSuggestionViewModel> SanPhamLienQuan { get; set; } = new();
        public bool ConHang => SoLuongTon > 0;
    }
}


