namespace SensorMonitor.Models
{
    public class Generator
    {
        public Guid SensorTelemetryId { get; set; }
        public required Guid Id { get; set; }
        public required int Voltage { get; set; }
        public required string Model { get; set; }
        public string? Description { get; set; }
        public required string Status { get; set; }
        public required int FuelLevel { get; set; }
        public required int TankSize { get; set; }
    }
}
