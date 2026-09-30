using SensorMonitor.Models;

namespace SensorMonitor.Database
{
    public interface ITelemetryRepository
    {
        Task InsertTelemetry(SensorTelemetry telemetry);
        Task InsertBatteryTelemetry(Battery battery);
        Task InsertGeneratorTelemetry(Generator generator);
        Task InsertRectifierTelemetry(Rectifier rectifier);
    }
}
