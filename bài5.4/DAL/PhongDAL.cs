using System.Data;
using System.Data.SqlClient;

namespace HotelPOS.DAL
{
    public class PhongDAL
    {
        public DataTable LayTatCaPhong()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT SoPhong, LoaiPhong, GiaTheoGio, TrangThai FROM Phong", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public bool CapNhatTrangThaiPhong(string soPhong, string trangThai)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string sql = "UPDATE Phong SET TrangThai = @TrangThai WHERE SoPhong = @SoPhong";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                cmd.Parameters.AddWithValue("@SoPhong", soPhong);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}