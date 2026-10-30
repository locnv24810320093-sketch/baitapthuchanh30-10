using System.Data.SqlClient;

namespace InventoryPOS.DAL
{
    public class DatabaseHelper
    {
        private static string strConn = @"Data Source=.\SQLEXPRESS;Initial Catalog=InventoryDB;Integrated Security=True;TrustServerCertificate=True";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(strConn);
        }
    }
}