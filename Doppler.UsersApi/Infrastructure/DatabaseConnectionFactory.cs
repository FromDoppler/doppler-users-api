using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Doppler.UsersApi.Infrastructure
{
    public class DatabaseConnectionFactory : IDatabaseConnectionFactory
    {
        private readonly string _connectionString;

        public DatabaseConnectionFactory(IOptions<DopplerDatabaseSettings> dopplerDataBaseSettings)
        {
            _connectionString = dopplerDataBaseSettings.Value.GetSqlConnectionString();
        }

        /// <summary>
        /// Open new connection and return it for use
        /// </summary>
        /// <returns></returns>
        public async Task<IDbConnection> GetConnection()
        {
            var cn = new SqlConnection(_connectionString);
            await cn.OpenAsync();
            return cn;
        }

        Task<IDbConnection> IDatabaseConnectionFactory.GetConnection()
        {
            throw new NotImplementedException();
        }
    }
}
