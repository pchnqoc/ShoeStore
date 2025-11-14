using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteGiay.Models;
using WebsiteGiay.Utilities;

namespace WebsiteGiay.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly AppDbContext _context;

        public CheckoutController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                TempData["Error"] = "Vui lòng đăng nhập để thanh toán.";
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Checkout") });
            }

            var user = await _context.NguoiDungs.FindAsync(userId.Value);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin người dùng.";
                return RedirectToAction("Index", "Cart");
            }

            var cart = await _context.GioHangs
                .Include(g => g.GioHangItems)
                    .ThenInclude(i => i.MaBienTheSpNavigation)
                        .ThenInclude(b => b.MaSanPhamNavigation)
                            .ThenInclude(sp => sp.MaThuongHieuNavigation)
                .Include(g => g.GioHangItems)
                    .ThenInclude(i => i.MaBienTheSpNavigation)
                        .ThenInclude(b => b.MaKichCoNavigation)
                .Include(g => g.GioHangItems)
                    .ThenInclude(i => i.MaBienTheSpNavigation)
                        .ThenInclude(b => b.MaMauNavigation)
                .FirstOrDefaultAsync(g => g.MaNguoiDung == userId.Value);

            if (cart == null || !cart.GioHangItems.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index", "Cart");
            }

            var viewModel = new CheckoutViewModel
            {
                MaNguoiDung = user.MaNguoiDung,
                HoTen = user.HoTen,
                DienThoai = user.DienThoai ?? string.Empty,
                DiaChiGiaoHang = user.DiaChi ?? string.Empty,
                PhuongThucThanhToan = "COD"
            };

            foreach (var item in cart.GioHangItems)
            {
                var product = item.MaBienTheSpNavigation.MaSanPhamNavigation;
                var variant = item.MaBienTheSpNavigation;
                
                viewModel.Items.Add(new CheckoutItemViewModel
                {
                    MaBienTheSp = variant.MaBienTheSp,
                    MaSanPham = product.MaSanPham,
                    TenSanPham = product.TenSanPham,
                    ThuongHieu = product.MaThuongHieuNavigation.TenThuongHieu,
                    ImageUrl = ImageHelper.ResolveImagePath(product.HinhAnh),
                    KichCo = variant.MaKichCoNavigation?.KichCo1 ?? string.Empty,
                    MauSac = variant.MaMauNavigation?.TenMau ?? string.Empty,
                    SoLuong = item.SoLuong,
                    DonGia = item.DonGia,
                    ThanhTien = item.DonGia * item.SoLuong
                });
            }

            viewModel.TongTien = viewModel.Items.Sum(i => i.ThanhTien);
            viewModel.PhiVanChuyen = 0; // Có thể tính phí vận chuyển dựa trên địa chỉ
            viewModel.TongCong = viewModel.TongTien + viewModel.PhiVanChuyen;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                TempData["Error"] = "Vui lòng đăng nhập để thanh toán.";
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Checkout") });
            }

            if (!ModelState.IsValid)
            {
                // Reload cart items if validation fails
                var cart = await _context.GioHangs
                    .Include(g => g.GioHangItems)
                        .ThenInclude(i => i.MaBienTheSpNavigation)
                            .ThenInclude(b => b.MaSanPhamNavigation)
                                .ThenInclude(sp => sp.MaThuongHieuNavigation)
                    .Include(g => g.GioHangItems)
                        .ThenInclude(i => i.MaBienTheSpNavigation)
                            .ThenInclude(b => b.MaKichCoNavigation)
                    .Include(g => g.GioHangItems)
                        .ThenInclude(i => i.MaBienTheSpNavigation)
                            .ThenInclude(b => b.MaMauNavigation)
                    .FirstOrDefaultAsync(g => g.MaNguoiDung == userId.Value);

                if (cart != null && cart.GioHangItems.Any())
                {
                    model.Items = cart.GioHangItems.Select(item =>
                    {
                        var product = item.MaBienTheSpNavigation.MaSanPhamNavigation;
                        var variant = item.MaBienTheSpNavigation;
                        return new CheckoutItemViewModel
                        {
                            MaBienTheSp = variant.MaBienTheSp,
                            MaSanPham = product.MaSanPham,
                            TenSanPham = product.TenSanPham,
                            ThuongHieu = product.MaThuongHieuNavigation.TenThuongHieu,
                            ImageUrl = ImageHelper.ResolveImagePath(product.HinhAnh),
                            KichCo = variant.MaKichCoNavigation?.KichCo1 ?? string.Empty,
                            MauSac = variant.MaMauNavigation?.TenMau ?? string.Empty,
                            SoLuong = item.SoLuong,
                            DonGia = item.DonGia,
                            ThanhTien = item.DonGia * item.SoLuong
                        };
                    }).ToList();

                    model.TongTien = model.Items.Sum(i => i.ThanhTien);
                    model.PhiVanChuyen = 0;
                    model.TongCong = model.TongTien + model.PhiVanChuyen;
                }

                return View("Index", model);
            }

            // Get cart
            var userCart = await _context.GioHangs
                .Include(g => g.GioHangItems)
                    .ThenInclude(i => i.MaBienTheSpNavigation)
                        .ThenInclude(b => b.MaSanPhamNavigation)
                .FirstOrDefaultAsync(g => g.MaNguoiDung == userId.Value);

            if (userCart == null || !userCart.GioHangItems.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index", "Cart");
            }

            // Validate stock availability - reload fresh data from database
            var cartItemIds = userCart.GioHangItems.Select(i => i.MaBienTheSp).ToList();
            var variants = await _context.BienTheSanPhams
                .Where(v => cartItemIds.Contains(v.MaBienTheSp))
                .ToListAsync();

            foreach (var cartItem in userCart.GioHangItems)
            {
                var variant = variants.FirstOrDefault(v => v.MaBienTheSp == cartItem.MaBienTheSp);

                if (variant == null)
                {
                    TempData["Error"] = $"Sản phẩm không tồn tại trong kho.";
                    return RedirectToAction("Index", "Cart");
                }

                if (variant.SoLuongTon < cartItem.SoLuong)
                {
                    var productName = cartItem.MaBienTheSpNavigation?.MaSanPhamNavigation?.TenSanPham ?? "N/A";
                    TempData["Error"] = $"Sản phẩm {productName} không đủ số lượng trong kho. Số lượng còn lại: {variant.SoLuongTon}";
                    return RedirectToAction("Index", "Checkout");
                }
            }

            // Create order
            var paymentMethodText = model.PhuongThucThanhToan == "COD" 
                ? "Thanh toán khi nhận hàng (COD)" 
                : "Chuyển khoản ngân hàng";
            
            var ghiChu = string.IsNullOrWhiteSpace(model.GhiChu) 
                ? $"Phương thức thanh toán: {paymentMethodText}" 
                : $"{model.GhiChu}\nPhương thức thanh toán: {paymentMethodText}";

            var donHang = new DonHang
            {
                MaNguoiDung = userId.Value,
                NgayDatHang = DateTime.Now,
                DiaChiGiaoHang = model.DiaChiGiaoHang,
                TongTien = model.TongCong,
                TrangThai = "Chờ xử lý",
                GhiChu = ghiChu
            };

            // Use transaction to ensure data consistency
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.DonHangs.Add(donHang);
                await _context.SaveChangesAsync();

                // Create order details - trigger TRG_CAPNHATTONKHO_INSERT will update stock automatically
                foreach (var cartItem in userCart.GioHangItems)
                {
                    var variant = variants.FirstOrDefault(v => v.MaBienTheSp == cartItem.MaBienTheSp);

                    if (variant != null && variant.SoLuongTon >= cartItem.SoLuong)
                    {
                        var chiTietDonHang = new ChiTietDonHang
                        {
                            MaDonHang = donHang.MaDonHang,
                            MaBienTheSp = cartItem.MaBienTheSp,
                            SoLuong = cartItem.SoLuong,
                            DonGia = cartItem.DonGia,
                            ThanhTien = cartItem.DonGia * cartItem.SoLuong
                        };

                        _context.ChiTietDonHangs.Add(chiTietDonHang);
                    }
                    else
                    {
                        // This should not happen if validation worked, but handle it anyway
                        await transaction.RollbackAsync();
                        TempData["Error"] = $"Sản phẩm không đủ số lượng trong kho. Vui lòng kiểm tra lại giỏ hàng.";
                        return RedirectToAction("Index", "Checkout");
                    }
                }

                // Save order details - trigger will update stock
                await _context.SaveChangesAsync();

                // Clear cart
                _context.GioHangItems.RemoveRange(userCart.GioHangItems);
                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = $"Có lỗi xảy ra khi đặt hàng: {ex.Message}";
                return RedirectToAction("Index", "Checkout");
            }

            // Update user info if changed
            var user = await _context.NguoiDungs.FindAsync(userId.Value);
            if (user != null)
            {
                if (!string.IsNullOrWhiteSpace(model.DienThoai) && user.DienThoai != model.DienThoai)
                {
                    user.DienThoai = model.DienThoai;
                }
                if (!string.IsNullOrWhiteSpace(model.DiaChiGiaoHang) && user.DiaChi != model.DiaChiGiaoHang)
                {
                    user.DiaChi = model.DiaChiGiaoHang;
                }
                await _context.SaveChangesAsync();
            }

            // Redirect to success page
            return RedirectToAction("Success", new { orderId = donHang.MaDonHang });
        }

        [HttpGet]
        public async Task<IActionResult> Success(int orderId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }

            var donHang = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(c => c.MaBienTheSpNavigation)
                        .ThenInclude(b => b.MaSanPhamNavigation)
                            .ThenInclude(sp => sp.MaThuongHieuNavigation)
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(c => c.MaBienTheSpNavigation)
                        .ThenInclude(b => b.MaKichCoNavigation)
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(c => c.MaBienTheSpNavigation)
                        .ThenInclude(b => b.MaMauNavigation)
                .FirstOrDefaultAsync(d => d.MaDonHang == orderId && d.MaNguoiDung == userId.Value);

            if (donHang == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("Index", "Home");
            }

            return View(donHang);
        }
    }
}

