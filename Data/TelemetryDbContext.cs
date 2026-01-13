using SensorMonitor.Models;
using Microsoft.EntityFrameworkCore;


namespace SensorMonitor.Data
{
    public class TelemetryDbContext(DbContextOptions<TelemetryDbContext> options) : DbContext(options)
    {
        public DbSet<SensorTelemetry> SensorTelemetries => Set<SensorTelemetry>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           modelBuilder.Entity<SensorTelemetry>()
                .ToTable("sensor_telemetries");
        }

    }
}
