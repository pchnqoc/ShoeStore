using System.Collections.Generic;
using System.Linq;

namespace WebsiteGiay.Models
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new();

        public decimal Subtotal => Items.Sum(i => i.ThanhTien);

        public decimal Shipping => 0m;

        public decimal Total => Subtotal + Shipping;
    }

    public class CartItemViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int MaGioHang { get; set; }
        public int MaBienTheSp { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public string ThuongHieu { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public List<string> Sizes { get; set; } = new();
        public string SelectedSize { get; set; } = string.Empty;
        public List<string> Colors { get; set; } = new();
        public string SelectedColor { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => DonGia * SoLuong;
    }
}


