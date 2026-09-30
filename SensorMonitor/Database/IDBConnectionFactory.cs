using System.Data;

namespace SensorMonitor.Database
{
    public interface IDBConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
