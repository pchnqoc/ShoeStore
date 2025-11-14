using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebsiteGiay.Models;
using WebsiteGiay.Utilities;

namespace WebsiteGiay.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? brand)
        {
            var viewModel = new ProductPageViewModel
            {
                DanhMucs = await _context.DanhMucGiays.OrderBy(dm => dm.TenDanhMuc).ToListAsync(),
                ThuongHieus = await _context.ThuongHieus.OrderBy(th => th.TenThuongHieu).ToListAsync(),
                KichCos = await _context.KichCos.OrderBy(kc => kc.KichCo1).ToListAsync(),
                MauSacs = await _context.MauSacs.OrderBy(ms => ms.TenMau).ToListAsync()
            };
            ViewBag.SelectedBrandId = brand?.ToString();

            var query = _context.SanPhams
                .Include(s => s.MaDanhMucNavigation)
                .Include(s => s.MaThuongHieuNavigation)
                .Include(s => s.BienTheSanPhams)
                    .ThenInclude(bt => bt.MaKichCoNavigation)
                .Include(s => s.BienTheSanPhams)
                    .ThenInclude(bt => bt.MaMauNavigation)
                .AsQueryable();

            if (brand.HasValue)
            {
                query = query.Where(s => s.MaThuongHieu == brand.Value);
            }

            var sanPhams = await query
                .OrderByDescending(s => s.NgayTao)
                .Take(40)
                .ToListAsync();

            viewModel.Products = sanPhams.Select(s =>
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
                    TenDanhMuc = s.MaDanhMucNavigation.TenDanhMuc,
                    TenThuongHieu = s.MaThuongHieuNavigation.TenThuongHieu,
                    Gia = s.Gia,
                    HinhAnh = ImageHelper.ResolveImagePath(s.HinhAnh),
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

            return View(viewModel);
        }

        [HttpGet]
        [Route("SanPham/ChiTiet/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.SanPhams
                .Include(s => s.MaDanhMucNavigation)
                .Include(s => s.MaThuongHieuNavigation)
                .Include(s => s.BienTheSanPhams)
                    .ThenInclude(bt => bt.MaKichCoNavigation)
                .Include(s => s.BienTheSanPhams)
                    .ThenInclude(bt => bt.MaMauNavigation)
                .FirstOrDefaultAsync(s => s.MaSanPham == id);

            if (product == null)
            {
                return NotFound();
            }

            var variants = product.BienTheSanPhams.ToList();
            var sizeItems = variants
                .GroupBy(v => new { v.MaKichCo, v.MaKichCoNavigation.KichCo1 })
                .Select(g => new SelectListItem
                {
                    Value = g.Key.MaKichCo.ToString(),
                    Text = g.Key.KichCo1,
                    Disabled = g.All(v => v.SoLuongTon <= 0)
                })
                .OrderBy(i => i.Text)
                .ToList();

            var colorItems = variants
                .GroupBy(v => new { v.MaMau, v.MaMauNavigation.TenMau })
                .Select(g => new SelectListItem
                {
                    Value = g.Key.MaMau.ToString(),
                    Text = g.Key.TenMau,
                    Disabled = g.All(v => v.SoLuongTon <= 0)
                })
                .OrderBy(i => i.Text)
                .ToList();

            var totalStock = variants.Sum(v => v.SoLuongTon);

            var relatedProductsRaw = await _context.SanPhams
                .Where(s => s.MaSanPham != product.MaSanPham &&
                            (s.MaDanhMuc == product.MaDanhMuc || s.MaThuongHieu == product.MaThuongHieu))
                .Include(s => s.BienTheSanPhams)
                    .ThenInclude(bt => bt.MaKichCoNavigation)
                .Include(s => s.BienTheSanPhams)
                    .ThenInclude(bt => bt.MaMauNavigation)
                .OrderByDescending(s => s.NgayTao)
                .Take(4)
                .ToListAsync();

            var relatedProducts = relatedProductsRaw.Select(s =>
            {
                var defaultVariant = s.BienTheSanPhams
                    .Where(v => v.SoLuongTon > 0)
                    .OrderByDescending(v => v.SoLuongTon)
                    .FirstOrDefault();

                return new ProductSuggestionViewModel
                {
                    MaSanPham = s.MaSanPham,
                    TenSanPham = s.TenSanPham,
                    Gia = s.Gia,
                    HinhAnh = ImageHelper.ResolveImagePath(s.HinhAnh),
                    DefaultBienTheId = defaultVariant?.MaBienTheSp,
                    DefaultSize = defaultVariant?.MaKichCoNavigation.KichCo1,
                    DefaultColor = defaultVariant?.MaMauNavigation.TenMau
                };
            }).ToList();

            var viewModel = new ProductDetailViewModel
            {
                MaSanPham = product.MaSanPham,
                TenSanPham = product.TenSanPham,
                MoTa = product.MoTa,
                Gia = product.Gia,
                HinhAnh = ImageHelper.ResolveImagePath(product.HinhAnh),
                DanhSachKichCo = sizeItems,
                DanhSachMau = colorItems,
                SoLuongTon = totalStock,
                SanPhamLienQuan = relatedProducts
            };

            ViewBag.BrandName = product.MaThuongHieuNavigation.TenThuongHieu;
            ViewBag.CategoryName = product.MaDanhMucNavigation.TenDanhMuc;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int sizeId, int colorId, int quantity = 1)
        {
            if (quantity <= 0)
            {
                TempData["Error"] = "Số lượng phải lớn hơn 0.";
                return RedirectToAction(nameof(Details), new { id = productId });
            }

            var product = await _context.SanPhams
                .Include(p => p.BienTheSanPhams)
                .FirstOrDefaultAsync(p => p.MaSanPham == productId);

            if (product == null)
            {
                TempData["Error"] = "Sản phẩm không tồn tại.";
                return RedirectToAction(nameof(Index));
            }

            var variant = await _context.BienTheSanPhams
                .Include(v => v.MaKichCoNavigation)
                .Include(v => v.MaMauNavigation)
                .FirstOrDefaultAsync(v =>
                    v.MaSanPham == productId &&
                    v.MaKichCo == sizeId &&
                    v.MaMau == colorId);

            if (variant == null)
            {
                TempData["Error"] = "Không tìm thấy biến thể phù hợp cho sản phẩm.";
                return RedirectToAction(nameof(Details), new { id = productId });
            }

            if (variant.SoLuongTon <= 0)
            {
                TempData["Error"] = "Biến thể sản phẩm đã hết hàng.";
                return RedirectToAction(nameof(Details), new { id = productId });
            }

            if (quantity > variant.SoLuongTon)
            {
                TempData["Error"] = $"Chỉ còn {variant.SoLuongTon} sản phẩm trong kho cho lựa chọn này.";
                return RedirectToAction(nameof(Details), new { id = productId });
            }

            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (!sessionUserId.HasValue)
            {
                TempData["Error"] = "Vui lòng đăng nhập trước khi thêm sản phẩm vào giỏ hàng.";
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action(nameof(Details), new { id = productId }) });
            }
            var userId = sessionUserId.Value;

            var cart = await _context.GioHangs
                .Include(g => g.GioHangItems)
                .FirstOrDefaultAsync(g => g.MaNguoiDung == userId);

            if (cart == null)
            {
                cart = new GioHang
                {
                    MaNguoiDung = userId,
                    NgayTao = DateTime.Now
                };
                _context.GioHangs.Add(cart);
                await _context.SaveChangesAsync();
            }

            var cartItem = cart.GioHangItems.FirstOrDefault(i => i.MaBienTheSp == variant.MaBienTheSp);
            if (cartItem == null)
            {
                cartItem = new GioHangItem
                {
                    MaGioHang = cart.MaGioHang,
                    MaBienTheSp = variant.MaBienTheSp,
                    SoLuong = quantity,
                    DonGia = product.Gia
                };
                _context.GioHangItems.Add(cartItem);
            }
            else
            {
                cartItem.SoLuong += quantity;
                cartItem.DonGia = product.Gia;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã thêm sản phẩm vào giỏ hàng!";
            return RedirectToAction("Index", "Cart");
        }
    }
}


