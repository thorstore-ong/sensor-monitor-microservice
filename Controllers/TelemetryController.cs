using SensorMonitor.Data;
using SensorMonitor.DTOs;
using SensorMonitor.Services;
using SensorMonitor.Validation;
using Microsoft.AspNetCore.Mvc;

namespace SensorMonitor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemeletryController(SensorService sensorService) : ControllerBase
    {
        private readonly SensorService _sensorService = sensorService;

        // POST api/Telemetry
        [HttpPost]
        public async Task<IActionResult> RecieveGatewayData([FromBody] GatewayDto gateway )
        {
            try
            {
                //declare validator object
                var validator = new TelemetryValidator();

                //Call validator method to validate JSON data
                validator.Validate(gateway);

                //declare normalizer to normalize JSON data
                var telemetryList = await _sensorService.HandlePost(gateway);

                return Created("", telemetryList);
            }
            catch(Exception ex) 
            { 
                return BadRequest(new {error = ex.Message});
            }

            //_sensorService.AddGateway(gateway);
            //return Ok( new { message = "Gateways and sensors have been added successfully."});
        }

        //get sensors by type
        [HttpGet("/sensors/{type}")]
        public async Task<IActionResult> GetSensorByType(string type)
        { 
            var sensors = await _sensorService.GetSensorsByTypeAsync( type);
             return Ok(sensors);
        }

        //get all sensors
        [HttpGet("/sensors")]
        public async Task<IActionResult> GetSensors()
        {
            var sensors = await _sensorService.GetAllSensorsAsync();
            return Ok(sensors);
        }


    }
}
