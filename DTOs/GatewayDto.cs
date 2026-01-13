

using System.Text.Json.Serialization;

namespace SensorMonitor.DTOs
{
    public class GatewayDto
    {
        [JsonPropertyName("gateway")]
        public string? GatewayId { get; set; }

        [JsonPropertyName("timestamp")]
        public string? Timestamp { get; set; }

        [JsonPropertyName("sensors")]
        public required List<SensorDto> Sensors { get; set;}
    }
}
