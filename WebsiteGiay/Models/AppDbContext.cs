using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebsiteGiay.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BaoCaoBanHang> BaoCaoBanHangs { get; set; }

    public virtual DbSet<BienTheSanPham> BienTheSanPhams { get; set; }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<DanhGia> DanhGia { get; set; }

    public virtual DbSet<DanhMucGiay> DanhMucGiays { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<GiamGia> GiamGia { get; set; }

    public virtual DbSet<GioHang> GioHangs { get; set; }

    public virtual DbSet<GioHangItem> GioHangItems { get; set; }

    public virtual DbSet<KhieuNai> KhieuNais { get; set; }

    public virtual DbSet<KichCo> KichCos { get; set; }

    public virtual DbSet<MauSac> MauSacs { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<PhanHoiKhieuNai> PhanHoiKhieuNais { get; set; }

    public virtual DbSet<PhanQuyen> PhanQuyens { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<ThuongHieu> ThuongHieus { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        => optionsBuilder.UseSqlServer("Server=LAPTOP-CVVJJQOL;Database=QLBanGiay;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BaoCaoBanHang>(entity =>
        {
            entity.HasKey(e => e.MaBaoCao).HasName("PK__BaoCaoBa__25A9188C9DCABAC4");
        });

        modelBuilder.Entity<BienTheSanPham>(entity =>
        {
            entity.HasKey(e => e.MaBienTheSp).HasName("PK__BienTheS__05DB678C653D90FA");

            entity.HasOne(d => d.MaKichCoNavigation).WithMany(p => p.BienTheSanPhams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BienTheSa__MaKic__52593CB8");

            entity.HasOne(d => d.MaMauNavigation).WithMany(p => p.BienTheSanPhams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BienTheSa__MaMau__534D60F1");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.BienTheSanPhams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BienTheSa__MaSan__5165187F");
        });

        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => new { e.MaDonHang, e.MaBienTheSp }).HasName("PK__ChiTietD__C2C832D512F5E206");

            entity.ToTable("ChiTietDonHang", tb =>
                {
                    tb.HasTrigger("TRG_CAPNHATTONKHO_DELETE");
                    tb.HasTrigger("TRG_CAPNHATTONKHO_INSERT");
                    tb.HasTrigger("TRG_CapNhatTongTien");
                });

            entity.HasOne(d => d.MaBienTheSpNavigation).WithMany(p => p.ChiTietDonHangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDo__MaBie__68487DD7");

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.ChiTietDonHangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDo__MaDon__6754599E");
        });

        modelBuilder.Entity<DanhGia>(entity =>
        {
            entity.HasKey(e => e.IdDanhGia).HasName("PK__DanhGia__81F722D2B2A15974");

            entity.Property(e => e.NgayDanhGia).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TrangThai).HasDefaultValueSql("((1))");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.DanhGia).HasConstraintName("FK__DanhGia__MaNguoi__6D0D32F4");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.DanhGia).HasConstraintName("FK__DanhGia__MaSanPh__6C190EBB");
        });

        modelBuilder.Entity<DanhMucGiay>(entity =>
        {
            entity.HasKey(e => e.MaDanhMuc).HasName("PK__DanhMucG__B3750887630D8B20");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.MaDonHang).HasName("PK__DonHang__129584AD17F2E727");

            entity.Property(e => e.NgayDatHang).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.DonHangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonHang__MaNguoi__5FB337D6");
        });

        modelBuilder.Entity<GiamGia>(entity =>
        {
            entity.HasKey(e => e.IdGiamGia).HasName("PK__GiamGia__E0F7D8B67E9AB529");

            entity.HasOne(d => d.MaDanhMucNavigation).WithMany(p => p.GiamGia).HasConstraintName("FK__GiamGia__MaDanhM__73BA3083");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.GiamGia)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__GiamGia__MaSanPh__72C60C4A");

            entity.HasOne(d => d.MaThuongHieuNavigation).WithMany(p => p.GiamGia).HasConstraintName("FK__GiamGia__MaThuon__74AE54BC");
        });

        modelBuilder.Entity<GioHang>(entity =>
        {
            entity.HasKey(e => e.MaGioHang).HasName("PK__GioHang__F5001DA3EA7BE781");

            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.GioHangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GioHang__MaNguoi__5629CD9C");
        });

        modelBuilder.Entity<GioHangItem>(entity =>
        {
            entity.HasKey(e => new { e.MaGioHang, e.MaBienTheSp }).HasName("PK__GioHangI__255DABDB27841444");

            entity.HasOne(d => d.MaBienTheSpNavigation).WithMany(p => p.GioHangItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GioHangIt__MaBie__5CD6CB2B");

            entity.HasOne(d => d.MaGioHangNavigation).WithMany(p => p.GioHangItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GioHangIt__MaGio__5BE2A6F2");
        });

        modelBuilder.Entity<KhieuNai>(entity =>
        {
            entity.HasKey(e => e.MaKhieuNai).HasName("PK__KhieuNai__1D72BE528EDEDC06");

            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TrangThai).HasDefaultValueSql("(N'Chờ xử lý')");

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.KhieuNais).HasConstraintName("FK__KhieuNai__MaDonH__7C4F7684");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.KhieuNaiMaNguoiDungNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KhieuNai__MaNguo__787EE5A0");

            entity.HasOne(d => d.NguoiXuLyNavigation).WithMany(p => p.KhieuNaiNguoiXuLyNavigations).HasConstraintName("FK__KhieuNai__NguoiX__7A672E12");

            entity.HasOne(d => d.Ma).WithMany(p => p.KhieuNais).HasConstraintName("FK__KhieuNai__7D439ABD");
        });

        modelBuilder.Entity<KichCo>(entity =>
        {
            entity.HasKey(e => e.MaKichCo).HasName("PK__KichCo__DE76ED877B2E7DB6");
        });

        modelBuilder.Entity<MauSac>(entity =>
        {
            entity.HasKey(e => e.MaMau).HasName("PK__MauSac__3A5BBB7DF78B1D2C");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK__NguoiDun__C539D76244C1949F");

            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaVaiTroNavigation).WithMany(p => p.NguoiDungs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NguoiDung__MaVai__3C69FB99");
        });

        modelBuilder.Entity<PhanHoiKhieuNai>(entity =>
        {
            entity.HasKey(e => e.IdPhanHoi).HasName("PK__PhanHoiK__D53EC95E44446B67");

            entity.Property(e => e.NgayPhanHoi).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaKhieuNaiNavigation).WithMany(p => p.PhanHoiKhieuNais)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhanHoiKh__MaKhi__00200768");

            entity.HasOne(d => d.MaNguoiTraLoiNavigation).WithMany(p => p.PhanHoiKhieuNais)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhanHoiKh__MaNgu__01142BA1");
        });

        modelBuilder.Entity<PhanQuyen>(entity =>
        {
            entity.HasKey(e => new { e.MaVaiTro, e.TenChucNang }).HasName("PK__PhanQuye__6EB17660203FB993");

            entity.Property(e => e.DuocTruyCap).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.MaVaiTroNavigation).WithMany(p => p.PhanQuyens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhanQuyen__MaVai__06CD04F7");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSanPham).HasName("PK__SanPham__FAC7442D555B1C38");

            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaDanhMucNavigation).WithMany(p => p.SanPhams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham__MaDanhM__44FF419A");

            entity.HasOne(d => d.MaThuongHieuNavigation).WithMany(p => p.SanPhams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham__MaThuon__45F365D3");
        });

        modelBuilder.Entity<ThuongHieu>(entity =>
        {
            entity.HasKey(e => e.MaThuongHieu).HasName("PK__ThuongHi__A3733E2CF55B7AC5");
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.MaVaiTro).HasName("PK__VaiTro__C24C41CF90F3BEE3");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
