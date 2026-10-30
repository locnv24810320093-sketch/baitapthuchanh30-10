using System;
using System.Data;
using HotelPOS.DAL;
using HotelPOS.DTO;

namespace HotelPOS.BUS
{
    public class HotelBUS
    {
        private PhongDAL phongDAL = new PhongDAL();
        private KhachHangDAL khDAL = new KhachHangDAL();
        private PhieuDatPhongDAL phieuDAL = new PhieuDatPhongDAL();

        public DataTable LayDanhSachPhong() => phongDAL.LayTatCaPhong();

        public KhachHangDTO TimKhachHangByCCCD(string cccd) => khDAL.TimTheoCCCD(cccd);

        public bool CheckIn(string soPhong, string hoTen, string cccd, string sdt, out string err)
        {
            err = string.Empty;
            if (string.IsNullOrWhiteSpace(hoTen) || string.IsNullOrWhiteSpace(cccd))
            {
                err = "Họ tên và CCCD không được để trống!";
                return false;
            }

            KhachHangDTO kh = khDAL.TimTheoCCCD(cccd);
            int maKH = kh != null ? kh.MaKH : khDAL.ThemKhachHang(new KhachHangDTO { HoTen = hoTen, CCCD = cccd, SoDienThoai = sdt });

            return phieuDAL.TaoPhieuDatPhong(soPhong, maKH) > 0;
        }

        public DataRow LayThongTinThuePhong(string soPhong) => phieuDAL.LayPhieuChuaThanhToanBySoPhong(soPhong);

        public void GoiDichVu(int maPhieu, int maDV, int soLuong) => phieuDAL.ThemSuDungDichVu(maPhieu, maDV, soLuong);

        public DataTable LayDichVuDaDung(int maPhieu) => phieuDAL.LayChiTietDichVuByMaPhieu(maPhieu);

        public bool CheckOut(int maPhieu, string soPhong, decimal tongTien)
        {
            return phieuDAL.ThanhToanCheckOut(maPhieu, soPhong, tongTien);
        }

        public bool ChuyenTrangThaiPhong(string soPhong, string trangThai)
        {
            return phongDAL.CapNhatTrangThaiPhong(soPhong, trangThai);
        }
    }
}