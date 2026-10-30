using System;
using System.Data;
using System.Data.SqlClient;

namespace CafePOS.DAL
{
    public class HoaDonDAL
    {
        public int LayMaHoaDonChuaThanhToanByBan(int maBan)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string sql = "SELECT MaHoaDon FROM HoaDon WHERE MaBan = @MaBan AND TrangThai = 0";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaBan", maBan);
                conn.Open();
                object res = cmd.ExecuteScalar();
                return res != null ? Convert.ToInt32(res) : -1;
            }
        }

        public int TaoHoaDonMoi(int maBan)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string sql = "INSERT INTO HoaDon (MaBan, GioVao, TongTien, TrangThai) OUTPUT INSERTED.MaHoaDon VALUES (@MaBan, GETDATE(), 0, 0)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaBan", maBan);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public DataTable LayChiTietHoaDonByMaHD(int maHoaDon)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string sql = @"SELECT ct.MaMon, m.TenMon, ct.SoLuong, ct.DonGia, (ct.SoLuong * ct.DonGia) AS ThanhTien
                               FROM ChiTietHoaDon ct
                               JOIN MonAn m ON ct.MaMon = m.MaMon
                               WHERE ct.MaHoaDon = @MaHoaDon";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue("@MaHoaDon", maHoaDon);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public void ThemChiTietHoaDon(int maHoaDon, int maMon, int soLuong, decimal donGia)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string checkSql = "SELECT SoLuong FROM ChiTietHoaDon WHERE MaHoaDon = @MaHoaDon AND MaMon = @MaMon";
                SqlCommand checkCmd = new SqlCommand(checkSql, conn);
                checkCmd.Parameters.AddWithValue("@MaHoaDon", maHoaDon);
                checkCmd.Parameters.AddWithValue("@MaMon", maMon);
                object res = checkCmd.ExecuteScalar();

                if (res != null)
                {
                    int slMoi = Convert.ToInt32(res) + soLuong;
                    if (slMoi > 0)
                    {
                        string updateSql = "UPDATE ChiTietHoaDon SET SoLuong = @SoLuong WHERE MaHoaDon = @MaHoaDon AND MaMon = @MaMon";
                        SqlCommand upCmd = new SqlCommand(updateSql, conn);
                        upCmd.Parameters.AddWithValue("@SoLuong", slMoi);
                        upCmd.Parameters.AddWithValue("@MaHoaDon", maHoaDon);
                        upCmd.Parameters.AddWithValue("@MaMon", maMon);
                        upCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string delSql = "DELETE FROM ChiTietHoaDon WHERE MaHoaDon = @MaHoaDon AND MaMon = @MaMon";
                        SqlCommand delCmd = new SqlCommand(delSql, conn);
                        delCmd.Parameters.AddWithValue("@MaHoaDon", maHoaDon);
                        delCmd.Parameters.AddWithValue("@MaMon", maMon);
                        delCmd.ExecuteNonQuery();
                    }
                }
                else if (soLuong > 0)
                {
                    string insertSql = "INSERT INTO ChiTietHoaDon (MaHoaDon, MaMon, SoLuong, DonGia) VALUES (@MaHoaDon, @MaMon, @SoLuong, @DonGia)";
                    SqlCommand insCmd = new SqlCommand(insertSql, conn);
                    insCmd.Parameters.AddWithValue("@MaHoaDon", maHoaDon);
                    insCmd.Parameters.AddWithValue("@MaMon", maMon);
                    insCmd.Parameters.AddWithValue("@SoLuong", soLuong);
                    insCmd.Parameters.AddWithValue("@DonGia", donGia);
                    insCmd.ExecuteNonQuery();
                }
            }
        }

        public bool ThanhToanHoaDon(int maHoaDon, int maBan, decimal tongTien)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    string sqlHD = "UPDATE HoaDon SET GioRa = GETDATE(), TongTien = @TongTien, TrangThai = 1 WHERE MaHoaDon = @MaHoaDon";
                    SqlCommand cmdHD = new SqlCommand(sqlHD, conn, trans);
                    cmdHD.Parameters.AddWithValue("@TongTien", tongTien);
                    cmdHD.Parameters.AddWithValue("@MaHoaDon", maHoaDon);
                    cmdHD.ExecuteNonQuery();

                    string sqlBan = "UPDATE BanAn SET TrangThai = N'Trống' WHERE MaBan = @MaBan";
                    SqlCommand cmdBan = new SqlCommand(sqlBan, conn, trans);
                    cmdBan.Parameters.AddWithValue("@MaBan", maBan);
                    cmdBan.ExecuteNonQuery();

                    trans.Commit();
                    return true;
                }
                catch
                {
                    trans.Rollback();
                    return false;
                }
            }
        }
    }
}