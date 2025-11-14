using System;
using System.Collections.Generic;

namespace WebsiteGiay.Models
{
    public class HomeViewModel
    {
        public List<SanPhamInfo> SanPhamMoiNhat { get; set; } = new();
        public List<SanPhamInfo> SanPhamNoiBat { get; set; } = new();
        public List<DanhMucGiay> DanhMucs { get; set; } = new();
        public List<ThuongHieu> ThuongHieus { get; set; } = new();
        public List<KichCo> KichCos { get; set; } = new();
        public List<MauSac> MauSacs { get; set; } = new();
        public int? SoLuongGioHang { get; set; }
        public decimal? TongTienGioHang { get; set; }
        public NguoiDung? NguoiDungHienTai { get; set; }
        public List<BrandHighlight> BrandHighlights { get; set; } = new();
    }

    public class SanPhamInfo
    {
        public int MaSanPham { get; set; }
        public int MaDanhMuc { get; set; }
        public int MaThuongHieu { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public string? HinhAnh { get; set; }
        public decimal Gia { get; set; }
        public decimal? GiaSauGiam { get; set; }
        public decimal? PhanTramGiam { get; set; }
        public double? DiemTrungBinh { get; set; }
        public int? SoLuongDanhGia { get; set; }
        public string TenDanhMuc { get; set; } = string.Empty;
        public string TenThuongHieu { get; set; } = string.Empty;
        public List<string> KichCoValues { get; set; } = new();
        public List<string> MauSacValues { get; set; } = new();
        public int? DefaultBienTheId { get; set; }
        public string? DefaultSize { get; set; }
        public string? DefaultColor { get; set; }
        public bool ConHang => DefaultBienTheId.HasValue;
    }

    public class BrandHighlight
    {
        public int MaThuongHieu { get; set; }
        public string TenThuongHieu { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public string? ImageUrl { get; set; }
    }
}

