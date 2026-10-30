using System.Data.SqlClient;

namespace HotelPOS.DAL
{
    public class DatabaseHelper
    {
        private static string strConn = @"Data Source=.\SQLEXPRESS;Initial Catalog=HotelDB;Integrated Security=True;TrustServerCertificate=True";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(strConn);
        }
    }
}