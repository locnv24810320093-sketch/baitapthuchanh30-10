using System.Data;
using System.Data.SqlClient;

namespace CafePOS.DAL
{
    public class BanAnDAL
    {
        public DataTable LayDanhSachBanAn()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM BanAn", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public void CapNhatTrangThaiBan(int maBan, string trangThai)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string sql = "UPDATE BanAn SET TrangThai = @TrangThai WHERE MaBan = @MaBan";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                cmd.Parameters.AddWithValue("@MaBan", maBan);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}