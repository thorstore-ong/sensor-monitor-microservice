namespace SensorMonitor.Models
{
    public class Rectifier
    {
        public Guid SensorTelemetryId { get; set; }
        public required Guid Id { get; set; }
        public required int Voltage { get; set; }
        public required string Model { get; set; }
        public string? Description { get; set; }
        public required int Current { get; set; }
        public required int LowVoltageDisconnect { get; set; }
        public required bool GridActive { get; set; }
    }
}
