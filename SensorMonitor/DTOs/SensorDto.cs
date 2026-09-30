using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SensorMonitor.DTOs
{
    public class SensorDto
    {
        [JsonPropertyName("sensorType")]
        public required string  SensorType { get; set; }

        [JsonPropertyName("sensorId")]
        public string? SensorId { get; set; }

        [JsonPropertyName("timestamp")]
        public string? Timestamp { get; set; }

        [JsonPropertyName("sensorData")]
        public JsonElement SensorData { get; set; }

   }
}
