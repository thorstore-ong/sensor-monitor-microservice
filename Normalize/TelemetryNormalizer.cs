using SensorMonitor.DTOs;
using SensorMonitor.Models;

namespace SensorMonitor.Normalize
{
    public class TelemetryNormalizer
    {
        public SensorTelemetry Normalize(GatewayDto gateway, SensorDto sensor) 
        {
           

            return new SensorTelemetry
            {
                GatewayId = string.IsNullOrEmpty(gateway.GatewayId) ? Guid.NewGuid().ToString() : gateway.GatewayId!,
                SensorId = string.IsNullOrEmpty(sensor.SensorId) ? Guid.NewGuid().ToString() : sensor.SensorId!,
                SensorType = sensor.SensorType!,
                Timestamp = ParseTimestamp(sensor.Timestamp ?? gateway.Timestamp),
                SensorData = sensor.SensorData
            };

        }

        //Private method that parses through timestamp, it either adds timestamp if null or converts string to DateTime tupe
        private static DateTime ParseTimestamp(string? timestamp)
        { 
            if (string.IsNullOrWhiteSpace(timestamp))
                return DateTime.UtcNow;

            if(DateTime.TryParse(timestamp, out var parsed))
                return DateTime.SpecifyKind(parsed, DateTimeKind.Utc);
            
            throw new Exception("Invalid timestamp format");        
        }
        
        //Normalizes the whole gateway telemetry
        public List<SensorTelemetry> NormalizeGateway(GatewayDto gateway) 
        {
            var result = new List<SensorTelemetry>();
            foreach(var sensor in gateway.Sensors)
            {
                result.Add(Normalize(gateway, sensor));
            }
            return result;
        }

    }
}
