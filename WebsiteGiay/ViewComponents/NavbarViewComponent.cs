using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteGiay.Models;

namespace WebsiteGiay.ViewComponents
{
    public class NavbarViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public NavbarViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var viewModel = new NavbarViewModel();

            // Lấy danh sách danh mục
            viewModel.DanhMucs = await _context.DanhMucGiays.ToListAsync();

            // Lấy thông tin người dùng và giỏ hàng
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                var gioHang = await _context.GioHangs
                    .Include(g => g.GioHangItems)
                    .FirstOrDefaultAsync(g => g.MaNguoiDung == userId.Value);

                if (gioHang != null)
                {
                    viewModel.SoLuongGioHang = gioHang.GioHangItems.Sum(gi => gi.SoLuong);
                    viewModel.TongTienGioHang = gioHang.GioHangItems.Sum(gi => gi.DonGia * gi.SoLuong);
                }

                viewModel.NguoiDung = await _context.NguoiDungs.FindAsync(userId.Value);
            }

            return View(viewModel);
        }
    }
}

