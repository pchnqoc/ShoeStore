namespace WebsiteGiay.Models
{
    public class ProductSuggestionViewModel
    {
        public int MaSanPham { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public decimal Gia { get; set; }
        public string? HinhAnh { get; set; }
        public int? DefaultBienTheId { get; set; }
        public string? DefaultSize { get; set; }
        public string? DefaultColor { get; set; }
        public bool ConHang => DefaultBienTheId.HasValue;
    }
}


