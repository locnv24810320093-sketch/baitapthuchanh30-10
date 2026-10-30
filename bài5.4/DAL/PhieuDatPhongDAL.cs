using System;
using System.Data;
using System.Data.SqlClient;

namespace HotelPOS.DAL
{
    public class PhieuDatPhongDAL
    {
        public int TaoPhieuDatPhong(string soPhong, int maKH)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    string sqlPhieu = "INSERT INTO PhieuDatPhong (SoPhong, MaKH, NgayNhanPhong) OUTPUT INSERTED.MaPhieu VALUES (@SoPhong, @MaKH, GETDATE())";
                    SqlCommand cmdPhieu = new SqlCommand(sqlPhieu, conn, trans);
                    cmdPhieu.Parameters.AddWithValue("@SoPhong", soPhong);
                    cmdPhieu.Parameters.AddWithValue("@MaKH", maKH);
                    int maPhieu = (int)cmdPhieu.ExecuteScalar();

                    string sqlPhong = "UPDATE Phong SET TrangThai = N'Đang ở' WHERE SoPhong = @SoPhong";
                    SqlCommand cmdPhong = new SqlCommand(sqlPhong, conn, trans);
                    cmdPhong.Parameters.AddWithValue("@SoPhong", soPhong);
                    cmdPhong.ExecuteNonQuery();

                    trans.Commit();
                    return maPhieu;
                }
                catch
                {
                    trans.Rollback();
                    return -1;
                }
            }
        }

        public DataRow LayPhieuChuaThanhToanBySoPhong(string soPhong)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string sql = @"SELECT TOP 1 p.MaPhieu, p.SoPhong, p.NgayNhanPhong, k.HoTen, k.CCCD, ph.GiaTheoGio
                               FROM PhieuDatPhong p
                               JOIN KhachHang k ON p.MaKH = k.MaKH
                               JOIN Phong ph ON p.SoPhong = ph.SoPhong
                               WHERE p.SoPhong = @SoPhong AND p.NgayTraPhong IS NULL
                               ORDER BY p.MaPhieu DESC";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue("@SoPhong", soPhong);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }

        public void ThemSuDungDichVu(int maPhieu, int maDV, int soLuong)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string sql = "INSERT INTO SuDungDichVu (MaPhieu, MaDV, SoLuong, ThoiGian) VALUES (@MaPhieu, @MaDV, @SoLuong, GETDATE())";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaPhieu", maPhieu);
                cmd.Parameters.AddWithValue("@MaDV", maDV);
                cmd.Parameters.AddWithValue("@SoLuong", soLuong);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public DataTable LayChiTietDichVuByMaPhieu(int maPhieu)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string sql = @"SELECT dv.TenDV, s.SoLuong, dv.DonGia, (s.SoLuong * dv.DonGia) AS ThanhTien, s.ThoiGian
                               FROM SuDungDichVu s
                               JOIN DichVu dv ON s.MaDV = dv.MaDV
                               WHERE s.MaPhieu = @MaPhieu";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue("@MaPhieu", maPhieu);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // Thực thi Transaction cho Check-out
        public bool ThanhToanCheckOut(int maPhieu, string soPhong, decimal tongTien)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    string sqlPhieu = "UPDATE PhieuDatPhong SET NgayTraPhong = GETDATE(), TongTienThanhToan = @TongTien WHERE MaPhieu = @MaPhieu";
                    SqlCommand cmdPhieu = new SqlCommand(sqlPhieu, conn, trans);
                    cmdPhieu.Parameters.AddWithValue("@TongTien", tongTien);
                    cmdPhieu.Parameters.AddWithValue("@MaPhieu", maPhieu);
                    cmdPhieu.ExecuteNonQuery();

                    string sqlPhong = "UPDATE Phong SET TrangThai = N'Đang dọn dẹp' WHERE SoPhong = @SoPhong";
                    SqlCommand cmdPhong = new SqlCommand(sqlPhong, conn, trans);
                    cmdPhong.Parameters.AddWithValue("@SoPhong", soPhong);
                    cmdPhong.ExecuteNonQuery();

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