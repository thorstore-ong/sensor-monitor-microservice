using SensorMonitor.Data;
using SensorMonitor.DTOs;
using SensorMonitor.Models;
using SensorMonitor.Normalize;
using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;

namespace SensorMonitor.Services
{
    public class SensorService(
        Channel<SensorTelemetry> channel,
        TelemetryNormalizer normalizer,
        TelemetryDbContext context)
    {

        private readonly TelemetryNormalizer _normalizer = normalizer;

        private readonly TelemetryDbContext _context = context;

        private readonly Channel<SensorTelemetry> _channel = channel;

        public async Task<List<SensorTelemetry>> HandlePost(GatewayDto gateway)
        {
            var telemetryList = _normalizer.NormalizeGateway(gateway);

            if (await _channel.Writer.WaitToWriteAsync())
            {
                foreach (var telemetry in telemetryList)
                {
                    await _channel.Writer.WriteAsync(telemetry);
                }
            }
            

            return telemetryList;
        }

        //Get all gateways
        public async Task <IEnumerable<SensorDto>> GetAllSensorsAsync()
        {
            return await _context.SensorTelemetries
                .Select(s => new SensorDto
                {
                    SensorId = s.SensorId,
                    SensorType = s.SensorType,
                    SensorData = s.SensorData
                })
                .ToListAsync();
        }

        //Get Sensors by type
        public async Task <IEnumerable<SensorDto>> GetSensorsByTypeAsync( string type )
        {
          
            return await _context.SensorTelemetries
                .Where(s => s.SensorType == type)
                .Select(s => new SensorDto
                {
                    SensorId = s.SensorId,
                    SensorType = s.SensorType,
                    SensorData = s.SensorData
                })
                .ToListAsync();
        }


    }
}
