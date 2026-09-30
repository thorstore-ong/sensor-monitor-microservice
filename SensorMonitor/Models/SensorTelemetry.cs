using System.Text.Json;
using System.ComponentModel.DataAnnotations;

namespace SensorMonitor.Models
{
    public class SensorTelemetry
    {
       
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string GatewayId { get; set; }

        public required string SensorType { get; set; }

        public required string SensorId { get; set; }

        public required DateTime Timestamp { get; set; }


    }
}
