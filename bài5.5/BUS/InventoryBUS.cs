using System.Collections.Generic;
using System.Data;
using InventoryPOS.DAL;
using InventoryPOS.DTO;

namespace InventoryPOS.BUS
{
    public class InventoryBUS
    {
        private VatTuDAL vatTuDAL = new VatTuDAL();
        private NhaCungCapDAL nccDAL = new NhaCungCapDAL();
        private PhieuKhoDAL phieuDAL = new PhieuKhoDAL();

        public DataTable LayDanhSachVatTu() => vatTuDAL.LayDanhSachVatTu();
        public DataTable TimKiemVatTu(string kw) => vatTuDAL.TimKiemVatTu(kw);
        public bool ThemVatTu(VatTuDTO vt) => vatTuDAL.ThemVatTu(vt);
        public bool SuaVatTu(VatTuDTO vt) => vatTuDAL.SuaVatTu(vt);
        public bool XoaVatTu(string ma) => vatTuDAL.XoaVatTu(ma);

        public DataTable LayDanhSachNCC() => nccDAL.LayDanhSachNCC();
        public bool ThemNCC(NhaCungCapDTO ncc) => nccDAL.ThemNCC(ncc);

        public bool LuuPhieuKho(PhieuKhoDTO phieu, List<ChiTietPhieuKhoDTO> list, out string err)
        {
            return phieuDAL.LuuPhieuKhoTransaction(phieu, list, out err);
        }

        public DataTable LayVatTuCanhBao(decimal nguong) => vatTuDAL.LayVatTuCanhBaoTon(nguong);
        public decimal TinhTongGiaTriTonKho() => vatTuDAL.TinhTongGiaTriTonKho();
    }
}