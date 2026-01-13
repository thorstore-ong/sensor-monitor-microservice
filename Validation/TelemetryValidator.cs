using SensorMonitor.DTOs;
using System.Text.Json;

namespace SensorMonitor.Validation
{
    public class TelemetryValidator
    {
        public  void Validate(GatewayDto gateway)
        {
            if (gateway == null)
                throw new Exception("Gateway telemetry is required");

            
            if (gateway.Sensors == null || gateway.Sensors.Count == 0)
                throw new Exception("At least one sensor is required");

            foreach (var sensor in gateway.Sensors)
            {
                ValidateSensor(sensor);
            }
            
        }

        private static void ValidateSensor(SensorDto sensor)
        {

            if (string.IsNullOrWhiteSpace(sensor.SensorType))
                throw new Exception("Sensor Type is required");

            if (sensor.SensorData.ValueKind == JsonValueKind.Undefined)
                throw new Exception("Sensor Data is required");

        }
    }
}
