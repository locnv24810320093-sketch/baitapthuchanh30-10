using System;
using System.Data;
using System.Data.SqlClient;
using InventoryPOS.DTO;

namespace InventoryPOS.DAL
{
    public class VatTuDAL
    {
        public DataTable LayDanhSachVatTu()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT MaVatTu, TenVatTu, DVT, SoLuongTon FROM VatTu", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // Chống SQL Injection bằng SqlParameter
        public DataTable TimKiemVatTu(string keyword)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string sql = "SELECT MaVatTu, TenVatTu, DVT, SoLuongTon FROM VatTu WHERE MaVatTu LIKE @kw OR TenVatTu LIKE @kw";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public bool ThemVatTu(VatTuDTO vt)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string sql = "INSERT INTO VatTu (MaVatTu, TenVatTu, DVT, SoLuongTon) VALUES (@Ma, @Ten, @DVT, @SL)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Ma", vt.MaVatTu);
                cmd.Parameters.AddWithValue("@Ten", vt.TenVatTu);
                cmd.Parameters.AddWithValue("@DVT", vt.DVT ?? "");
                cmd.Parameters.AddWithValue("@SL", vt.SoLuongTon);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool SuaVatTu(VatTuDTO vt)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string sql = "UPDATE VatTu SET TenVatTu = @Ten, DVT = @DVT, SoLuongTon = @SL WHERE MaVatTu = @Ma";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Ma", vt.MaVatTu);
                cmd.Parameters.AddWithValue("@Ten", vt.TenVatTu);
                cmd.Parameters.AddWithValue("@DVT", vt.DVT ?? "");
                cmd.Parameters.AddWithValue("@SL", vt.SoLuongTon);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool XoaVatTu(string maVatTu)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string sql = "DELETE FROM VatTu WHERE MaVatTu = @Ma";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Ma", maVatTu);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Cảnh báo tồn kho dưới định mức
        public DataTable LayVatTuCanhBaoTon(decimal ngudong)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string sql = "SELECT MaVatTu, TenVatTu, DVT, SoLuongTon FROM VatTu WHERE SoLuongTon <= @Nguong";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Nguong", ngudong);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // Tính tổng giá trị kho bằng ExecuteScalar
        public decimal TinhTongGiaTriTonKho()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string sql = @"SELECT ISNULL(SUM(v.SoLuongTon * ISNULL((SELECT TOP 1 DonGia FROM ChiTietPhieuKho WHERE MaVatTu = v.MaVatTu ORDER BY MaPhieu DESC), 100000)), 0) 
                               FROM VatTu v";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                object res = cmd.ExecuteScalar();
                return Convert.ToDecimal(res);
            }
        }
    }
}