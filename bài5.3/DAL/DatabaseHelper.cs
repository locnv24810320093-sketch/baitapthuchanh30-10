using System.Data.SqlClient;

namespace CafePOS.DAL
{
    public static class DatabaseHelper
    {
        private static string strConn = @"Data Source=.\SQLEXPRESS;Initial Catalog=CafePOSDB;Integrated Security=True;TrustServerCertificate=True";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(strConn);
        }
    }
}