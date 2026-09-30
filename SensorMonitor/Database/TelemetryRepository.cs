using Dapper;
using SensorMonitor.Models;

namespace SensorMonitor.Database
{
    public class TelemetryRepository(IDBConnectionFactory connectionFactory) : ITelemetryRepository
    {
        private readonly IDBConnectionFactory _connectionFactory = connectionFactory;
        
        public async Task InsertTelemetry(SensorTelemetry telemetry) 
        { 
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"
                INSERT INTO sensor_telemetry (id, gateway_id, sensor_id, sensor_type, timestamp)
                VALUES (@Id, @GatewayId, @SensorId, @SensorType, @Timestamp);"
                ;

            await conn.ExecuteAsync(sql, telemetry);
        }

        public async Task InsertBatteryTelemetry(Battery battery)
        {
            using var conn = _connectionFactory.CreateConnection(); ;
            var sql = @"
                INSERT INTO battery_sensor (id, sensor_telemetry_id, voltage, model, description, current, state_of_charge, capacity)
                VALUES (@Id, @SensorTelemetryId, @Voltage, @Model, @Description, @Current, @StateOfCharge, @Capacity);"
                ;

            await conn.ExecuteAsync(sql, battery);
        }

        public async Task InsertGeneratorTelemetry(Generator generator)
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"
                INSERT INTO generator_sensor (id, sensor_telemetry_id, voltage, model, description, status, fuel_level, tank_size)
                VALUES (@Id, @SensorTelemetryId, @Voltage, @Model, @Description, @Status, @FuelLevel, @TankSize);"
                ;

            await conn.ExecuteAsync(sql, generator);
        }

        public async Task InsertRectifierTelemetry(Rectifier rectifier)
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"
                INSERT INTO rectifier_sensor (id, sensor_telemetry_id, voltage, model, description, current, low_voltage_disconnect, grid_active)
                VALUES (@Id, @SensorTelemetryId, @Voltage, @Model, @Description, @Current, @LowVoltageDisconnect, @GridActive);"
                ;

            await conn.ExecuteAsync(sql, rectifier);
        }


    }
}
