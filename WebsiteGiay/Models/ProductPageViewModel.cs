using System.Collections.Generic;

namespace WebsiteGiay.Models
{
    public class ProductPageViewModel
    {
        public List<SanPhamInfo> Products { get; set; } = new();
        public List<DanhMucGiay> DanhMucs { get; set; } = new();
        public List<ThuongHieu> ThuongHieus { get; set; } = new();
        public List<KichCo> KichCos { get; set; } = new();
        public List<MauSac> MauSacs { get; set; } = new();
    }
}


