using Microsoft.Data.SqlClient;

namespace HRMS_Common
{
    public class AppDb
    {
        public SqlConnection Connection { get; }
        public int CommandTimeOut { get; }
        public AppDb(string? connectionString, int commandTimeOut)
        {
            Connection = new SqlConnection(connectionString);
            CommandTimeOut = commandTimeOut;
        }
        public void dispose() => Connection.Dispose();
    }
}
