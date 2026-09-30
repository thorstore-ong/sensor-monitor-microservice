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

            if(string.IsNullOrWhiteSpace(gateway.GatewayId))
                throw new Exception("Gateway ID is required");


            if (gateway.Sensors == null || gateway.Sensors.Count == 0)
                throw new Exception("At least one sensor is required");

            foreach (var sensor in gateway.Sensors)
            {
                try
                {
                    ValidateSensor(sensor);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Sensor validation failed: {ex.Message}"); //added a try-catch block if you want to handle individual sensor validation errors
                }
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
