using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteGiay.Models;
using WebsiteGiay.Utilities;

namespace WebsiteGiay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel();

            // Lấy 3 sản phẩm mới nhất cho banner
            var sanPhamMoiNhat = await _context.SanPhams
                .Include(s => s.MaDanhMucNavigation)
                .Include(s => s.MaThuongHieuNavigation)
                .Include(s => s.GiamGia)
                .Include(s => s.DanhGia)
                .Include(s => s.BienTheSanPhams)
                    .ThenInclude(bt => bt.MaKichCoNavigation)
                .Include(s => s.BienTheSanPhams)
                    .ThenInclude(bt => bt.MaMauNavigation)
                .OrderByDescending(s => s.NgayTao)
                .Take(3)
                .ToListAsync();

            viewModel.SanPhamMoiNhat = sanPhamMoiNhat.Select(s =>
            {
                var defaultVariant = s.BienTheSanPhams
                    .Where(v => v.SoLuongTon > 0)
                    .OrderByDescending(v => v.SoLuongTon)
                    .FirstOrDefault();

                return new SanPhamInfo
                {
                    MaSanPham = s.MaSanPham,
                    MaDanhMuc = s.MaDanhMuc,
                    MaThuongHieu = s.MaThuongHieu,
                    TenSanPham = s.TenSanPham,
                    HinhAnh = ImageHelper.ResolveImagePath(s.HinhAnh),
                    Gia = s.Gia,
                    TenDanhMuc = s.MaDanhMucNavigation.TenDanhMuc,
                    TenThuongHieu = s.MaThuongHieuNavigation.TenThuongHieu,
                    DiemTrungBinh = s.DanhGia.Where(d => d.TrangThai == true && d.DiemSo.HasValue)
                        .Average(d => (double?)d.DiemSo),
                    SoLuongDanhGia = s.DanhGia.Count(d => d.TrangThai == true),
                    GiaSauGiam = CalculateGiaSauGiam(s),
                    PhanTramGiam = GetPhanTramGiam(s),
                    KichCoValues = s.BienTheSanPhams
                        .Select(bt => bt.MaKichCoNavigation.KichCo1)
                        .Distinct()
                        .ToList(),
                    MauSacValues = s.BienTheSanPhams
                        .Select(bt => bt.MaMauNavigation.TenMau)
                        .Distinct()
                        .ToList(),
                    DefaultBienTheId = defaultVariant?.MaBienTheSp,
                    DefaultSize = defaultVariant?.MaKichCoNavigation.KichCo1,
                    DefaultColor = defaultVariant?.MaMauNavigation.TenMau
                };
            }).ToList();

            // Lấy 8 sản phẩm nổi bật (gần đây)
            var sanPhamNoiBat = await _context.SanPhams
                .Include(s => s.MaDanhMucNavigation)
                .Include(s => s.MaThuongHieuNavigation)
                .Include(s => s.GiamGia)
                .Include(s => s.DanhGia)
                .Include(s => s.BienTheSanPhams)
                    .ThenInclude(bt => bt.MaKichCoNavigation)
                .Include(s => s.BienTheSanPhams)
                    .ThenInclude(bt => bt.MaMauNavigation)
                .OrderByDescending(s => s.NgayTao)
                .Take(8)
                .ToListAsync();

            viewModel.SanPhamNoiBat = sanPhamNoiBat.Select(s =>
            {
                var defaultVariant = s.BienTheSanPhams
                    .Where(v => v.SoLuongTon > 0)
                    .OrderByDescending(v => v.SoLuongTon)
                    .FirstOrDefault();

                return new SanPhamInfo
                {
                    MaSanPham = s.MaSanPham,
                    MaDanhMuc = s.MaDanhMuc,
                    MaThuongHieu = s.MaThuongHieu,
                    TenSanPham = s.TenSanPham,
                    HinhAnh = ImageHelper.ResolveImagePath(s.HinhAnh),
                    Gia = s.Gia,
                    TenDanhMuc = s.MaDanhMucNavigation.TenDanhMuc,
                    TenThuongHieu = s.MaThuongHieuNavigation.TenThuongHieu,
                    DiemTrungBinh = s.DanhGia.Where(d => d.TrangThai == true && d.DiemSo.HasValue)
                        .Average(d => (double?)d.DiemSo),
                    SoLuongDanhGia = s.DanhGia.Count(d => d.TrangThai == true),
                    GiaSauGiam = CalculateGiaSauGiam(s),
                    PhanTramGiam = GetPhanTramGiam(s),
                    KichCoValues = s.BienTheSanPhams
                        .Select(bt => bt.MaKichCoNavigation.KichCo1)
                        .Distinct()
                        .ToList(),
                    MauSacValues = s.BienTheSanPhams
                        .Select(bt => bt.MaMauNavigation.TenMau)
                        .Distinct()
                        .ToList(),
                    DefaultBienTheId = defaultVariant?.MaBienTheSp,
                    DefaultSize = defaultVariant?.MaKichCoNavigation.KichCo1,
                    DefaultColor = defaultVariant?.MaMauNavigation.TenMau
                };
            }).ToList();

            // Lấy danh sách danh mục
            viewModel.DanhMucs = await _context.DanhMucGiays.ToListAsync();

            // Lấy danh sách thương hiệu
            viewModel.ThuongHieus = await _context.ThuongHieus.ToListAsync();

            // Lấy danh sách kích cỡ
            viewModel.KichCos = await _context.KichCos.ToListAsync();

            // Lấy danh sách màu sắc
            viewModel.MauSacs = await _context.MauSacs.ToListAsync();

            // Lấy thương hiệu nổi bật cùng ảnh đại diện sản phẩm
            var brandHighlightsRaw = await _context.ThuongHieus
                .OrderBy(th => th.TenThuongHieu)
                .Select(th => new
                {
                    th.MaThuongHieu,
                    th.TenThuongHieu,
                    th.MoTa,
                    Image = th.SanPhams
                        .OrderByDescending(sp => sp.NgayTao)
                        .Select(sp => sp.HinhAnh)
                        .FirstOrDefault()
                })
                .Take(4)
                .ToListAsync();

            viewModel.BrandHighlights = brandHighlightsRaw
                .Select(b => new BrandHighlight
                {
                    MaThuongHieu = b.MaThuongHieu,
                    TenThuongHieu = b.TenThuongHieu,
                    MoTa = b.MoTa,
                    ImageUrl = ImageHelper.ResolveImagePath(b.Image)
                })
                .ToList();

            // Lấy thông tin giỏ hàng (tạm thời dùng session, có thể cải thiện sau)
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                var gioHang = await _context.GioHangs
                    .Include(g => g.GioHangItems)
                        .ThenInclude(gi => gi.MaBienTheSpNavigation)
                    .FirstOrDefaultAsync(g => g.MaNguoiDung == userId.Value);

                if (gioHang != null)
                {
                    viewModel.SoLuongGioHang = gioHang.GioHangItems.Sum(gi => gi.SoLuong);
                    viewModel.TongTienGioHang = gioHang.GioHangItems.Sum(gi => gi.DonGia * gi.SoLuong);
                }

                viewModel.NguoiDungHienTai = await _context.NguoiDungs.FindAsync(userId.Value);
            }

            return View(viewModel);
        }

        private decimal? CalculateGiaSauGiam(SanPham sanPham)
        {
            var giamGiaHienTai = sanPham.GiamGia
                .Where(g => g.MaSanPham == sanPham.MaSanPham &&
                           g.NgayBatDau <= DateTime.Now &&
                           g.NgayKetThuc >= DateTime.Now)
                .FirstOrDefault();

            if (giamGiaHienTai == null) return null;

            if (giamGiaHienTai.PhanTramGiam.HasValue)
            {
                return sanPham.Gia * (1 - giamGiaHienTai.PhanTramGiam.Value / 100);
            }
            else if (giamGiaHienTai.GiaTriGiam.HasValue)
            {
                return Math.Max(0, sanPham.Gia - giamGiaHienTai.GiaTriGiam.Value);
            }

            return null;
        }

        private decimal? GetPhanTramGiam(SanPham sanPham)
        {
            var giamGiaHienTai = sanPham.GiamGia
                .Where(g => g.MaSanPham == sanPham.MaSanPham &&
                           g.NgayBatDau <= DateTime.Now &&
                           g.NgayKetThuc >= DateTime.Now)
                .FirstOrDefault();

            return giamGiaHienTai?.PhanTramGiam;
        }


        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> FilterProducts([FromBody] FilterRequest request)
        {
            var query = _context.SanPhams
                .Include(s => s.MaDanhMucNavigation)
                .Include(s => s.MaThuongHieuNavigation)
                .Include(s => s.GiamGia)
                .Include(s => s.DanhGia)
                .Include(s => s.BienTheSanPhams)
                    .ThenInclude(bt => bt.MaKichCoNavigation)
                .Include(s => s.BienTheSanPhams)
                    .ThenInclude(bt => bt.MaMauNavigation)
                .AsQueryable();

            if (request != null && request.DanhMuc.HasValue)
            {
                query = query.Where(s => s.MaDanhMuc == request.DanhMuc.Value);
            }

            if (request != null && request.ThuongHieu.HasValue)
            {
                query = query.Where(s => s.MaThuongHieu == request.ThuongHieu.Value);
            }

            if (request != null && request.KichCo.HasValue)
            {
                query = query.Where(s => s.BienTheSanPhams.Any(bt => bt.MaKichCo == request.KichCo.Value));
            }

            if (request != null && request.MauSac.HasValue)
            {
                query = query.Where(s => s.BienTheSanPhams.Any(bt => bt.MaMau == request.MauSac.Value));
            }

            var sanPhams = await query
                .OrderByDescending(s => s.NgayTao)
                .Take(8)
                .ToListAsync();

            var result = sanPhams.Select(s => new SanPhamInfo
            {
                MaSanPham = s.MaSanPham,
                MaDanhMuc = s.MaDanhMuc,
                MaThuongHieu = s.MaThuongHieu,
                TenSanPham = s.TenSanPham,
                HinhAnh = ImageHelper.ResolveImagePath(s.HinhAnh),
                Gia = s.Gia,
                TenDanhMuc = s.MaDanhMucNavigation.TenDanhMuc,
                TenThuongHieu = s.MaThuongHieuNavigation.TenThuongHieu,
                DiemTrungBinh = s.DanhGia.Where(d => d.TrangThai == true && d.DiemSo.HasValue)
                    .Average(d => (double?)d.DiemSo),
                SoLuongDanhGia = s.DanhGia.Count(d => d.TrangThai == true),
                GiaSauGiam = CalculateGiaSauGiam(s),
                PhanTramGiam = GetPhanTramGiam(s),
                KichCoValues = s.BienTheSanPhams
                    .Select(bt => bt.MaKichCoNavigation.KichCo1)
                    .Distinct()
                    .ToList(),
                MauSacValues = s.BienTheSanPhams
                    .Select(bt => bt.MaMauNavigation.TenMau)
                    .Distinct()
                    .ToList()
            }).ToList();

            return PartialView("_ProductGrid", result);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
