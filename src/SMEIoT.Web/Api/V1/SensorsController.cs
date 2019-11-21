using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NodaTime;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;
using SMEIoT.Web.ApiModels;
using SMEIoT.Web.BindingModels;

namespace SMEIoT.Web.Api.V1
{
  public class SensorsController : BaseController 
  {
    private readonly ILogger<SensorsController> _logger;
    private readonly ISensorService _service;
    private readonly IMqttSensorService _mqttService;


    public SensorsController(ILogger<SensorsController> logger, ISensorService service, IMqttSensorService mqttService)
    {
      _logger = logger;
      _service = service;
      _mqttService = mqttService;
    }

    [HttpGet("{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SensorDetailsApiModel>> Show(string name)
    {
      var sensor = await _service.GetSensorByName(name);
      var values = new List<double>();
      await foreach (var val in _service.GetSensorValues(sensor.Name, SystemClock.Instance.GetCurrentInstant(), Duration.FromSeconds(5)))
      {
        values.Add(val);
      }
      var res = new SensorDetailsApiModel(sensor, new SensorValuesApiModel(values, SystemClock.Instance.GetCurrentInstant(), Duration.FromSeconds(5)));

      return Ok(res);
    }

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BasicSensorApiModel>> Create(SensorLocatorBindingModel view)
    {
      await _service.CreateSensor(view.Name);
      var sensor = await _service.GetSensorByName(view.Name);
      var res = new BasicSensorApiModel(sensor);
      return CreatedAtAction(nameof(Create), res);
    }

    [HttpGet("candidates")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<ActionResult<SensorCandidatesApiModel>> ListCandidates()
    {
      var sensor = _mqttService.ListSensorNames("dummy");
      return Ok(new SensorCandidatesApiModel { Names = sensor });
    }


  }
}
