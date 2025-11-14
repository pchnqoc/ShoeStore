namespace WebsiteGiay.Models
{
    public class NavbarViewModel
    {
        public List<DanhMucGiay> DanhMucs { get; set; } = new();
        public int SoLuongGioHang { get; set; }
        public decimal TongTienGioHang { get; set; }
        public NguoiDung? NguoiDung { get; set; }
    }
}

