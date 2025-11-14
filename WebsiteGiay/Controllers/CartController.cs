using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteGiay.Models;
using WebsiteGiay.Utilities;

namespace WebsiteGiay.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Cart") });
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

            var model = new CartViewModel();

            if (cart != null)
            {
                var productIds = cart.GioHangItems
                    .Select(i => i.MaBienTheSpNavigation.MaSanPham)
                    .Distinct()
                    .ToList();

                var variants = await _context.BienTheSanPhams
                    .Include(v => v.MaKichCoNavigation)
                    .Include(v => v.MaMauNavigation)
                    .Where(v => productIds.Contains(v.MaSanPham))
                    .ToListAsync();

                var variantsLookup = variants
                    .GroupBy(v => v.MaSanPham)
                    .ToDictionary(g => g.Key, g => g.ToList());

                foreach (var item in cart.GioHangItems)
                {
                    var product = item.MaBienTheSpNavigation.MaSanPhamNavigation;
                    variantsLookup.TryGetValue(product.MaSanPham, out var productVariants);
                    productVariants ??= new List<BienTheSanPham>();

                    var selectedSize = item.MaBienTheSpNavigation.MaKichCoNavigation?.KichCo1 ?? string.Empty;
                    var selectedColor = item.MaBienTheSpNavigation.MaMauNavigation?.TenMau ?? string.Empty;

                    var sizes = productVariants
                        .Select(v => v.MaKichCoNavigation.KichCo1)
                        .Distinct()
                        .OrderBy(k => k)
                        .ToList();

                    if (!string.IsNullOrWhiteSpace(selectedSize) && !sizes.Contains(selectedSize))
                    {
                        sizes.Insert(0, selectedSize);
                    }

                    var colors = productVariants
                        .Select(v => v.MaMauNavigation.TenMau)
                        .Distinct()
                        .OrderBy(m => m)
                        .ToList();

                    if (!string.IsNullOrWhiteSpace(selectedColor) && !colors.Contains(selectedColor))
                    {
                        colors.Insert(0, selectedColor);
                    }

                    model.Items.Add(new CartItemViewModel
                    {
                        Id = model.Items.Count + 1,
                        ProductId = product.MaSanPham,
                        MaGioHang = item.MaGioHang,
                        MaBienTheSp = item.MaBienTheSp,
                        TenSanPham = product.TenSanPham,
                        ThuongHieu = product.MaThuongHieuNavigation.TenThuongHieu,
                        ImageUrl = ImageHelper.ResolveImagePath(product.HinhAnh),
                        DonGia = item.DonGia,
                        SoLuong = item.SoLuong,
                        Sizes = sizes,
                        SelectedSize = selectedSize,
                        Colors = colors,
                        SelectedColor = selectedColor
                    });
                }
            }

            return View(model);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Add([FromBody] AddToCartRequest request)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return Unauthorized(new { redirectUrl = Url.Action("Login", "Account", new { returnUrl = Request.Headers["Referer"].ToString() }) });
            }

            if (request.Quantity < 1)
            {
                request.Quantity = 1;
            }

            var product = await _context.SanPhams
                .Include(p => p.BienTheSanPhams)
                    .ThenInclude(v => v.MaKichCoNavigation)
                .Include(p => p.BienTheSanPhams)
                    .ThenInclude(v => v.MaMauNavigation)
                .FirstOrDefaultAsync(p => p.MaSanPham == request.ProductId);

            if (product == null)
            {
                return NotFound(new { message = "Sản phẩm không tồn tại." });
            }

            var variant = await ResolveVariantAsync(product, request);
            if (variant == null)
            {
                return BadRequest(new { message = "Không tìm thấy biến thể phù hợp cho sản phẩm." });
            }

            var cart = await _context.GioHangs
                .Include(g => g.GioHangItems)
                .FirstOrDefaultAsync(g => g.MaNguoiDung == userId.Value);

            if (cart == null)
            {
                cart = new GioHang
                {
                    MaNguoiDung = userId.Value,
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
                    SoLuong = request.Quantity,
                    DonGia = product.Gia
                };
                _context.GioHangItems.Add(cartItem);
            }
            else
            {
                cartItem.SoLuong += request.Quantity;
                cartItem.DonGia = product.Gia;
            }

            await _context.SaveChangesAsync();

            var totalQuantity = await _context.GioHangItems
                .Where(i => i.MaGioHang == cart.MaGioHang)
                .SumAsync(i => i.SoLuong);

            return Ok(new
            {
                message = "Đã thêm sản phẩm vào giỏ hàng!",
                totalQuantity
            });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdateItem([FromBody] UpdateCartItemRequest request)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return Unauthorized(new { redirectUrl = Url.Action("Login", "Account", new { returnUrl = Url.Action("Index", "Cart") }) });
            }

            var cart = await _context.GioHangs
                .Include(g => g.GioHangItems)
                    .ThenInclude(i => i.MaBienTheSpNavigation)
                        .ThenInclude(b => b.MaSanPhamNavigation)
                            .ThenInclude(p => p.BienTheSanPhams)
                                .ThenInclude(v => v.MaKichCoNavigation)
                .Include(g => g.GioHangItems)
                    .ThenInclude(i => i.MaBienTheSpNavigation)
                        .ThenInclude(b => b.MaSanPhamNavigation)
                            .ThenInclude(p => p.BienTheSanPhams)
                                .ThenInclude(v => v.MaMauNavigation)
                .FirstOrDefaultAsync(g => g.MaNguoiDung == userId.Value);

            if (cart == null)
            {
                return NotFound(new { message = "Không tìm thấy giỏ hàng." });
            }

            var item = cart.GioHangItems.FirstOrDefault(i => i.MaBienTheSp == request.MaBienTheSp);
            if (item == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm trong giỏ hàng." });
            }

            var product = item.MaBienTheSpNavigation.MaSanPhamNavigation;

            if (request.Quantity.HasValue && request.Quantity.Value > 0)
            {
                item.SoLuong = request.Quantity.Value;
            }

            var targetVariantId = item.MaBienTheSp;
            BienTheSanPham? targetVariant = null;
            if (!string.IsNullOrWhiteSpace(request.Size) || !string.IsNullOrWhiteSpace(request.Color))
            {
                targetVariant = product.BienTheSanPhams.FirstOrDefault(v =>
                    (string.IsNullOrWhiteSpace(request.Size) || v.MaKichCoNavigation.KichCo1.Equals(request.Size, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrWhiteSpace(request.Color) || v.MaMauNavigation.TenMau.Equals(request.Color, StringComparison.OrdinalIgnoreCase)));

                if (targetVariant != null && targetVariant.MaBienTheSp != item.MaBienTheSp)
                {
                    targetVariantId = targetVariant.MaBienTheSp;
                    var existingItem = cart.GioHangItems.FirstOrDefault(i => i.MaBienTheSp == targetVariantId);
                    if (existingItem != null)
                    {
                        existingItem.SoLuong += item.SoLuong;
                        existingItem.DonGia = product.Gia;
                        _context.GioHangItems.Remove(item);
                    }
                    else
                    {
                        _context.GioHangItems.Remove(item);
                        _context.GioHangItems.Add(new GioHangItem
                        {
                            MaGioHang = cart.MaGioHang,
                            MaBienTheSp = targetVariantId,
                            SoLuong = request.Quantity ?? item.SoLuong,
                            DonGia = product.Gia
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();

            var updatedItem = await _context.GioHangItems
                .Include(i => i.MaBienTheSpNavigation)
                    .ThenInclude(v => v.MaKichCoNavigation)
                .Include(i => i.MaBienTheSpNavigation)
                    .ThenInclude(v => v.MaMauNavigation)
                .FirstOrDefaultAsync(i => i.MaGioHang == cart.MaGioHang && i.MaBienTheSp == targetVariantId);

            if (updatedItem == null)
            {
                return Ok(new { subtotal = 0m, total = 0m, itemTotal = 0m, quantity = 0, newBienTheSp = targetVariantId });
            }

            var subtotal = await _context.GioHangItems
                .Where(i => i.MaGioHang == cart.MaGioHang)
                .SumAsync(i => i.DonGia * i.SoLuong);

            return Ok(new
            {
                message = "Cập nhật giỏ hàng thành công.",
                subtotal,
                total = subtotal,
                itemTotal = updatedItem.SoLuong * updatedItem.DonGia,
                quantity = updatedItem.SoLuong,
                unitPrice = updatedItem.DonGia,
                selectedSize = updatedItem.MaBienTheSpNavigation.MaKichCoNavigation.KichCo1,
                selectedColor = updatedItem.MaBienTheSpNavigation.MaMauNavigation.TenMau,
                newBienTheSp = updatedItem.MaBienTheSp
            });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Remove([FromBody] RemoveCartItemRequest request)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return Unauthorized(new { redirectUrl = Url.Action("Login", "Account", new { returnUrl = Url.Action("Index", "Cart") }) });
            }

            var cart = await _context.GioHangs
                .Include(g => g.GioHangItems)
                .FirstOrDefaultAsync(g => g.MaNguoiDung == userId.Value);

            if (cart == null)
            {
                return NotFound(new { message = "Không tìm thấy giỏ hàng." });
            }

            var item = cart.GioHangItems.FirstOrDefault(i => i.MaBienTheSp == request.MaBienTheSp);
            if (item == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm trong giỏ hàng." });
            }

            _context.GioHangItems.Remove(item);
            await _context.SaveChangesAsync();

            var subtotal = await _context.GioHangItems
                .Where(i => i.MaGioHang == cart.MaGioHang)
                .SumAsync(i => i.DonGia * i.SoLuong);

            var totalQuantity = await _context.GioHangItems
                .Where(i => i.MaGioHang == cart.MaGioHang)
                .SumAsync(i => i.SoLuong);

            return Ok(new
            {
                message = "Đã xóa sản phẩm khỏi giỏ hàng.",
                subtotal,
                total = subtotal,
                totalQuantity
            });
        }

        private async Task<BienTheSanPham?> ResolveVariantAsync(SanPham product, AddToCartRequest request)
        {
            BienTheSanPham? variant = null;

            if (request.BienTheSpId.HasValue)
            {
                variant = product.BienTheSanPhams.FirstOrDefault(v => v.MaBienTheSp == request.BienTheSpId.Value);
            }

            if (variant == null && (!string.IsNullOrWhiteSpace(request.Size) || !string.IsNullOrWhiteSpace(request.Color)))
            {
                variant = product.BienTheSanPhams.FirstOrDefault(v =>
                    (string.IsNullOrWhiteSpace(request.Size) || v.MaKichCoNavigation.KichCo1.Equals(request.Size, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrWhiteSpace(request.Color) || v.MaMauNavigation.TenMau.Equals(request.Color, StringComparison.OrdinalIgnoreCase)));
            }

            if (variant == null)
            {
                variant = product.BienTheSanPhams.FirstOrDefault();
            }

            if (variant == null)
            {
                variant = await _context.BienTheSanPhams
                    .Include(v => v.MaKichCoNavigation)
                    .Include(v => v.MaMauNavigation)
                    .FirstOrDefaultAsync(v => v.MaSanPham == product.MaSanPham);
            }

            return variant;
        }
    }

    public class AddToCartRequest
    {
        public int ProductId { get; set; }
        public int? BienTheSpId { get; set; }
        public int Quantity { get; set; } = 1;
        public string? Size { get; set; }
        public string? Color { get; set; }
    }

    public class UpdateCartItemRequest
    {
        public int MaBienTheSp { get; set; }
        public int? Quantity { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
    }

    public class RemoveCartItemRequest
    {
        public int MaBienTheSp { get; set; }
    }
}


