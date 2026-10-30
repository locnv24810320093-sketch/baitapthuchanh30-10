using System.Data;
using System.Data.SqlClient;
using InventoryPOS.DTO;

namespace InventoryPOS.DAL
{
    public class NhaCungCapDAL
    {
        public DataTable LayDanhSachNCC()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT MaNCC, TenNCC, DienThoai, DiaChi FROM NhaCungCap", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public bool ThemNCC(NhaCungCapDTO ncc)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string sql = "INSERT INTO NhaCungCap (TenNCC, DienThoai, DiaChi) VALUES (@Ten, @DT, @DC)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Ten", ncc.TenNCC);
                cmd.Parameters.AddWithValue("@DT", ncc.DienThoai ?? "");
                cmd.Parameters.AddWithValue("@DC", ncc.DiaChi ?? "");
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}