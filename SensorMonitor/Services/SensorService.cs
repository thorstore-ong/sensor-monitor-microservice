using SensorMonitor.DTOs;
using SensorMonitor.Models;
using SensorMonitor.Normalize;
using System.Threading.Channels;
using SensorMonitor.Database;
using Dapper;


namespace SensorMonitor.Services
{
    public class SensorService(
        Channel<object> channel,
        TelemetryNormalizer normalizer,
        IDBConnectionFactory connectionFactory)
    {
        private readonly TelemetryNormalizer _normalizer = normalizer;

        private readonly Channel<object> _channel = channel; //depedency injection not needed

        private readonly IDBConnectionFactory _connectionFactory = connectionFactory;

        public async Task<List<SensorTelemetry>> HandlePost(GatewayDto gateway)
        {
            var telemetryList = _normalizer.NormalizeGateway(gateway);

            // Parse all sensor data first, so a bad payload doesn't leave orphan telemetry rows
            var sensorDataList = new List<object>();
            for (var i = 0; i < gateway.Sensors.Count; i++)
            {
                var sensorData = _normalizer.NormalizeSensorData(gateway.Sensors[i]);
                var telemetryId = telemetryList[i].Id;

                switch (sensorData)
                {
                    case Battery b: b.SensorTelemetryId = telemetryId; break;
                    case Generator g: g.SensorTelemetryId = telemetryId; break;
                    case Rectifier r: r.SensorTelemetryId = telemetryId; break;
                }
                sensorDataList.Add(sensorData);
            }

            // Telemetry rows must be queued first because the sensor tables reference them
            foreach (var telemetry in telemetryList)
                await _channel.Writer.WriteAsync(telemetry);

            foreach (var sensorData in sensorDataList)
                await _channel.Writer.WriteAsync(sensorData);

            return telemetryList;
        }

        //Get all gateways
        public async Task <IEnumerable<SensorTelemetry>> GetAllSensorsAsync()
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"SELECT * FROM sensor_telemetry;";
            return await conn.QueryAsync<SensorTelemetry>(sql);
        }

        //Get Sensors by type
        public async Task <IEnumerable<SensorTelemetry>> GetSensorsByIdAsync( string gateway_id, string sensor_id, DateTime time )
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"SELECT * FROM sensor_telemetry
                WHERE gateway_id = @GatewayId
                  AND sensor_id = @SensorId
                  AND timestamp = @Timestamp;";
            return await conn.QueryAsync<SensorTelemetry>(sql, new
            {
                GatewayId = gateway_id,
                SensorId = sensor_id,
                Timestamp = time
            }
            );
        }

        public async Task<IEnumerable<Battery>> GetBatteriesAsync()
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"SELECT * FROM battery_sensor;";
            return await conn.QueryAsync<Battery>(sql);
        }

        public async Task<IEnumerable<Generator>> GetGeneratorsAsync()
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"SELECT * FROM generator_sensor;";
            return await conn.QueryAsync<Generator>(sql);
        }

        public async Task<IEnumerable<Rectifier>> GetRectifiersAsync()
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"SELECT * FROM rectifier_sensor;";
            return await conn.QueryAsync<Rectifier>(sql);
        }

    }
}
