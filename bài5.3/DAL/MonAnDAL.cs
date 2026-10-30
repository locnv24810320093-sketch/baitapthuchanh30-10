using System.Data;
using System.Data.SqlClient;

namespace CafePOS.DAL
{
    public class MonAnDAL
    {
        public DataTable LayDanhSachDanhMuc()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM DanhMuc", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public DataTable LayMonAnTheoDanhMuc(int maDanhMuc)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string sql = "SELECT * FROM MonAn WHERE MaDanhMuc = @MaDanhMuc";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue("@MaDanhMuc", maDanhMuc);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public DataTable LayTatCaMonAn()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string sql = @"SELECT m.MaMon, m.TenMon, d.TenDanhMuc, m.MaDanhMuc, m.DonGia 
                               FROM MonAn m JOIN DanhMuc d ON m.MaDanhMuc = d.MaDanhMuc";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public bool ThemMon(string tenMon, int maDanhMuc, decimal donGia)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string sql = "INSERT INTO MonAn (TenMon, MaDanhMuc, DonGia) VALUES (@TenMon, @MaDanhMuc, @DonGia)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@TenMon", tenMon);
                cmd.Parameters.AddWithValue("@MaDanhMuc", maDanhMuc);
                cmd.Parameters.AddWithValue("@DonGia", donGia);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool SuaMon(int maMon, string tenMon, int maDanhMuc, decimal donGia)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string sql = "UPDATE MonAn SET TenMon = @TenMon, MaDanhMuc = @MaDanhMuc, DonGia = @DonGia WHERE MaMon = @MaMon";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaMon", maMon);
                cmd.Parameters.AddWithValue("@TenMon", tenMon);
                cmd.Parameters.AddWithValue("@MaDanhMuc", maDanhMuc);
                cmd.Parameters.AddWithValue("@DonGia", donGia);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool XoaMon(int maMon)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string sql = "DELETE FROM MonAn WHERE MaMon = @MaMon";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaMon", maMon);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}