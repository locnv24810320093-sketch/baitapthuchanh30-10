using System.Data;
using CafePOS.DAL;
using CafePOS.DTO;

namespace CafePOS.BUS
{
    public class MonAnBUS
    {
        private MonAnDAL monAnDAL = new MonAnDAL();

        public DataTable LayTatCaMonAn() => monAnDAL.LayTatCaMonAn();
        public DataTable LayDanhSachDanhMuc() => monAnDAL.LayDanhSachDanhMuc();
        public DataTable LayMonAnTheoDanhMuc(int maDanhMuc) => monAnDAL.LayMonAnTheoDanhMuc(maDanhMuc);

        public bool ThemMon(MonAnDTO mon, out string error)
        {
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(mon.TenMon))
            {
                error = "Tên món ăn không được để trống!";
                return false;
            }
            if (mon.DonGia < 0)
            {
                error = "Đơn giá không được âm!";
                return false;
            }
            return monAnDAL.ThemMon(mon.TenMon, mon.MaDanhMuc, mon.DonGia);
        }

        public bool SuaMon(MonAnDTO mon, out string error)
        {
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(mon.TenMon))
            {
                error = "Tên món ăn không được để trống!";
                return false;
            }
            if (mon.DonGia < 0)
            {
                error = "Đơn giá không được âm!";
                return false;
            }
            return monAnDAL.SuaMon(mon.MaMon, mon.TenMon, mon.MaDanhMuc, mon.DonGia);
        }

        public bool XoaMon(int maMon) => monAnDAL.XoaMon(maMon);
    }
}