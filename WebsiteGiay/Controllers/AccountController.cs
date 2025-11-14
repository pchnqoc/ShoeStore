using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteGiay.Models;
using WebsiteGiay.Models.Auth;

namespace WebsiteGiay.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["Title"] = "Đăng Nhập";
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["Title"] = "Đăng Nhập";
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var input = (model.UserNameOrEmail ?? string.Empty).Trim();
            // Tìm user theo username hoặc email (bỏ khoảng trắng, mặc định collation đã case-insensitive)
            var user = await _context.NguoiDungs
                .FirstOrDefaultAsync(u => u.TenDangNhap == input || u.Email == input);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập/Email hoặc mật khẩu không đúng.");
                return View(model);
            }

            // Chấp nhận cả mật khẩu đã mã hoá SHA256 và legacy plain-text (dữ liệu cũ)
            var inputHash = HashPassword(model.Password);
            var passwordOk = user.MatKhau == inputHash || user.MatKhau == model.Password;
            if (!passwordOk)
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập/Email hoặc mật khẩu không đúng.");
                return View(model);
            }

            // Lưu thông tin vào Session
            HttpContext.Session.SetInt32("UserId", user.MaNguoiDung);
            HttpContext.Session.SetString("UserName", user.HoTen);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewData["Title"] = "Đăng Ký";
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ViewData["Title"] = "Đăng Ký";
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kiểm tra trùng username/email
            var existed = await _context.NguoiDungs
                .AnyAsync(u => u.TenDangNhap == model.TenDangNhap || u.Email == model.Email);
            if (existed)
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc Email đã tồn tại.");
                return View(model);
            }

            // Lấy role 'User'
            var userRole = await _context.VaiTros.FirstOrDefaultAsync(r => r.TenVaiTro == "User");
            if (userRole == null)
            {
                // Nếu chưa có thì tạo nhanh role User
                userRole = new VaiTro { TenVaiTro = "User" };
                _context.VaiTros.Add(userRole);
                await _context.SaveChangesAsync();
            }

            var user = new NguoiDung
            {
                TenDangNhap = model.TenDangNhap,
                Email = model.Email,
                MatKhau = HashPassword(model.MatKhau),
                HoTen = model.HoTen,
                NgaySinh = model.NgaySinh,
                GioiTinh = model.GioiTinh,
                DienThoai = model.DienThoai,
                DiaChi = model.DiaChi,
                MaVaiTro = userRole.MaVaiTro,
                NgayTao = DateTime.Now
            };

            _context.NguoiDungs.Add(user);
            await _context.SaveChangesAsync();

            // Auto login
            HttpContext.Session.SetInt32("UserId", user.MaNguoiDung);
            HttpContext.Session.SetString("UserName", user.HoTen);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("UserName");
            return RedirectToAction("Index", "Home");
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            var sb = new StringBuilder();
            foreach (var b in hash)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        [HttpGet]
        public IActionResult CheckAuth()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            return Json(new
            {
                isAuthenticated = userId.HasValue,
                userId
            });
        }
    }
}


