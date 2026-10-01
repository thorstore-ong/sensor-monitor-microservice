using System.Threading.Channels;
using SensorMonitor.Models;
using SensorMonitor.Database;



namespace SensorMonitor.Services
{
    public class TelemetryProcessor(Channel<object> channel, IServiceScopeFactory scopeFactory) : BackgroundService
    {
        private readonly Channel<object> _channel = channel;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)

        {
           
            await foreach (var item in _channel.Reader.ReadAllAsync())
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var _repo = scope.ServiceProvider.GetRequiredService<ITelemetryRepository>();
                    {
                        switch (item)
                        {
                            case SensorTelemetry telemetry:
                                await _repo.InsertTelemetry(telemetry);
                                break;

                            case Battery battery:
                                await _repo.InsertBatteryTelemetry(battery);
                                break;
                            case Generator generator:
                                await _repo.InsertGeneratorTelemetry(generator);
                                break;
                            case Rectifier rectifier:
                                await _repo.InsertRectifierTelemetry(rectifier);
                                break;

                            default:
                                throw new InvalidOperationException($"Unknown sensor data type: {item.GetType().Name}");
                        }
                    }
                
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to store {item.GetType().Name}: {ex.Message}");
                }
            }
        }
    }
}
