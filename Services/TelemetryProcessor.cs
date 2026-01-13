using SensorMonitor.Data;
using System.Threading.Channels;
using SensorMonitor.Models;



namespace SensorMonitor.Services
{
    public class TelemetryProcessor(Channel<SensorTelemetry> channel, IServiceScopeFactory scopeFactory) : BackgroundService
    {
        private readonly Channel<SensorTelemetry> _channel = channel;
        private readonly IServiceScopeFactory _scopeFactory =scopeFactory;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var telemetry in _channel.Reader.ReadAllAsync(stoppingToken))
            {
                using var scope = _scopeFactory.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<TelemetryDbContext>();
                try {
                    _context.SensorTelemetries.Add(telemetry);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing telemetry: {ex.Message}");
                }
                await _context.SaveChangesAsync(stoppingToken);
            }
        }
    }
}
