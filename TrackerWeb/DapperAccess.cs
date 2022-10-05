using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace TrackerWeb
{
    public class DapperAccess : IDisposable
    {
        private SqlConnection conn { get; set; }

        public DapperAccess(IConfiguration _configuration){
            conn = new SqlConnection(_configuration["ConnectionStrings:tracker"]);
            conn.Open();
        }

        public void Dispose()
        {
            conn.Close();
            conn.Dispose();
        }

        internal int Execute(string query, object? parm = null,int timeout = 30)
        {
            return conn.Execute(query, parm, commandTimeout: timeout);
        }

        internal List<T> GetSimpleData<T>(string query, object? parm = null, int timeout = 30)
        {
            return conn.Query<T>(query, parm, commandTimeout: timeout).ToList();
        }

        internal string GetJSON(string query, object? parm = null, int timeout = 30)
        {
            return conn.Query<string>(query + " FOR JSON AUTO", parm, commandTimeout: timeout).First();
        }

    }
}
