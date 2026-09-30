using Dapper;
using Microsoft.AspNetCore.Identity;
using Npgsql;

namespace SensorMonitor.Database
{
    public class DatabaseInitializer(IDBConnectionFactory connectionFactory, IConfiguration configuration )
    {
        private readonly IDBConnectionFactory _connectionFactory = connectionFactory;
        private readonly IConfiguration _configuration = configuration;

        public async Task InitializeAsync()
        {
            await EnsureDatabaseExistsAsync(_configuration);
            await InitializeTablesAsync();
        }
        public async Task EnsureDatabaseExistsAsync(IConfiguration configuration)
        {
            const string databaseName = "TelemetryDB";

            var adminConnectionString =
                configuration.GetConnectionString("PostgresAdmin");

            await using var conn = new NpgsqlConnection(adminConnectionString);
            await conn.OpenAsync();

            // Check if the database already exists
            await using (var checkCmd = conn.CreateCommand())
            {
                checkCmd.CommandText = """
            SELECT 1
            FROM pg_database
            WHERE datname = @dbName;
        """;

                checkCmd.Parameters.AddWithValue("dbName", databaseName);

                var exists = await checkCmd.ExecuteScalarAsync();

                if (exists != null)
                    return;
            }

            // Create the database 
            await using (var createCmd = conn.CreateCommand())
            {
                createCmd.CommandText = $"""
            CREATE DATABASE "{databaseName}";
        """;

                await createCmd.ExecuteNonQueryAsync();
            }
        }

        public async Task InitializeTablesAsync() { 
            using var conn = _connectionFactory.CreateConnection();
            conn.Open();

            var createSensorTelemetryTable = @"
            CREATE TABLE IF NOT EXISTS sensor_telemetry (
                id UUID PRIMARY KEY,
                gateway_id TEXT NOT NULL,
                sensor_id TEXT NOT NULL,
                sensor_type TEXT NOT NULL,
                timestamp TIMESTAMPTZ NOT NULL
            );";

            var createBatteryTelemeryTable = @"
            CREATE TABLE IF NOT EXISTS battery_sensor (
                id UUID PRIMARY KEY,
                sensor_telemetry_id UUID REFERENCES sensor_telemetry(id),
                voltage INT,
                model TEXT NOT NULL,
                description TEXT,
                current INT,
                state_of_charge INT,
                capacity INT
            );";

            var createGeneratorTelemeryTable = @"
            CREATE TABLE IF NOT EXISTS generator_sensor (
                id UUID PRIMARY KEY,
                sensor_telemetry_id UUID REFERENCES sensor_telemetry(id),
                voltage INT,
                model TEXT NOT NULL,
                description TEXT,
                status TEXT NOT NULL,
                fuel_level INT,
                tank_size INT
            );";

            var createRectifierTelemeryTable = @"
            CREATE TABLE IF NOT EXISTS rectifier_sensor (
                id UUID PRIMARY KEY,
                sensor_telemetry_id UUID REFERENCES sensor_telemetry(id),
                voltage INT,
                model TEXT NOT NULL,
                description TEXT,
                current INT,
                low_voltage_disconnect INT,
                grid_active BOOLEAN NOT NULL DEFAULT TRUE
            );";

            await conn.ExecuteAsync(createSensorTelemetryTable);
            await conn.ExecuteAsync(createBatteryTelemeryTable);
            await conn.ExecuteAsync(createGeneratorTelemeryTable);
            await conn.ExecuteAsync(createRectifierTelemeryTable);




        }
    }
}
