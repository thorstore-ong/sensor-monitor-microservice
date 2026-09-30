
using System.ComponentModel.DataAnnotations.Schema;

namespace SensorMonitor.Models
{
    public class Battery
    {
        public required Guid Id { get; set; }
        public required int Voltage { get; set; }
        public required string Model { get; set; }
        public string? Description { get; set; }
        public required int Current {  get; set; }
        public required int StateOfCharge { get; set; }
        public required int Capacity { get; set; }

    }
}
