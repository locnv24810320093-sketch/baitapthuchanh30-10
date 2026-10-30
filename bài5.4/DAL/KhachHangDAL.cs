using System;
using System.Data.SqlClient;
using HotelPOS.DTO;

namespace HotelPOS.DAL
{
    public class KhachHangDAL
    {
        public KhachHangDTO TimTheoCCCD(string cccd)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string sql = "SELECT MaKH, HoTen, CCCD, SoDienThoai FROM KhachHang WHERE CCCD = @CCCD";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@CCCD", cccd);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new KhachHangDTO
                        {
                            MaKH = Convert.ToInt32(reader["MaKH"]),
                            HoTen = reader["HoTen"].ToString(),
                            CCCD = reader["CCCD"].ToString(),
                            SoDienThoai = reader["SoDienThoai"].ToString()
                        };
                    }
                }
            }
            return null;
        }

        public int ThemKhachHang(KhachHangDTO kh)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string sql = "INSERT INTO KhachHang (HoTen, CCCD, SoDienThoai) OUTPUT INSERTED.MaKH VALUES (@HoTen, @CCCD, @SoDienThoai)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@HoTen", kh.HoTen);
                cmd.Parameters.AddWithValue("@CCCD", kh.CCCD);
                cmd.Parameters.AddWithValue("@SoDienThoai", kh.SoDienThoai ?? "");
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }
    }
}