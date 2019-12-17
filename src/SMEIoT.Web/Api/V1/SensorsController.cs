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
    private readonly ILogger _logger;
    private readonly IDeviceService _service;
    private readonly IMqttIdentifierService _mqttService;

    public SensorsController(ILogger<SensorsController> logger, IDeviceService service, IMqttIdentifierService mqttService)
    {
      _logger = logger;
      _service = service;
      _mqttService = mqttService;
    }

    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<SensorDetailsApiModelList>> Index([FromQuery] int start = 1, [FromQuery] int limit = 10)
    {
      var list = new List<SensorDetailsApiModel>();
      await foreach (var sensor in _service.ListSensorsAsync(start, limit))
      {
        var vals = new List<double>();
        await foreach (var v in _service.GetSensorValuesByDeviceAndSensorAsync(sensor.Device, sensor, SystemClock.Instance.GetCurrentInstant()-Duration.FromSeconds(5), Duration.FromSeconds(5)))
        {
          vals.Add(v);
        }
        var values = new SensorValuesApiModel(vals, SystemClock.Instance.GetCurrentInstant()-Duration.FromSeconds(5), Duration.FromSeconds(5));
        list.Add(new SensorDetailsApiModel(sensor, values));
      }

      return Ok(new SensorDetailsApiModelList(list));
    }

    [HttpGet("{deviceName}/{sensorName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SensorDetailsApiModel>> Show(string deviceName, string sensorName)
    {
      var device = await _service.GetDeviceByNameAsync(deviceName);
      var sensor = await _service.GetSensorByDeviceAndNameAsync(device, sensorName);   
      var values = new List<double>();
      values.Add(1.0);
      values.Add(2.0);
      values.Add(3.0);
      values.Add(4.0);
      values.Add(5.0);

      await foreach (var val in _service.GetSensorValuesByDeviceAndSensorAsync(device, sensor, SystemClock.Instance.GetCurrentInstant(), Duration.FromSeconds(5)))
      {
        _logger.LogTrace($"add into list {val}");
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
      var device = await _service.GetDeviceByNameAsync(view.DeviceName);
      await _service.CreateSensorByDeviceAndNameAsync(device, view.Name);
      var sensor = await _service.GetSensorByDeviceAndNameAsync(device, view.Name);
      var res = new BasicSensorApiModel(sensor);
      return CreatedAtAction(nameof(Create), res);
    }
  }
}
