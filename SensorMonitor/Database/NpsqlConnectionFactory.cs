using Npgsql;
using System.Data;


namespace SensorMonitor.Database
{
    public class NpsqlConnectionFactory : IDBConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public NpsqlConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
