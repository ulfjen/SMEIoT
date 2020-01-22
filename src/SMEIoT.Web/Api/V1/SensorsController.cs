using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Text;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;
using SMEIoT.Core.Exceptions;
using SMEIoT.Web.ApiModels;
using SMEIoT.Web.BindingModels;
using System.ComponentModel;
using System;

namespace SMEIoT.Web.Api.V1
{
  public class SensorsController : BaseController 
  {
    private readonly ILogger _logger;
    private readonly IDeviceService _service;
    private readonly ISensorValueService _valueService;
    private readonly IClock _clock;

    public SensorsController(
      ILogger<SensorsController> logger,
      IDeviceService service,
      IClock clock,
      ISensorValueService valueService)
    {
      _logger = logger;
      _service = service;
      _clock = clock;
      _valueService = valueService;
    }

    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [Authorize]
    public async Task<ActionResult<SensorDetailsApiModelList>> Index([FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
      var list = new List<SensorDetailsApiModel>();
      var sensors = new Dictionary<long, Sensor>();
      await foreach (var sensor in _service.ListSensorsAsync(offset, limit))
      {
        sensors[sensor.Id] = sensor;
      }
      
      var valsBySensorId = new Dictionary<long, List<(double, Instant)>>();
      await foreach (var v in _valueService.GetLastNumberOfValuesBySensorsAsync(sensors.Values, 10))
      {
        if (!valsBySensorId.ContainsKey(v.sensor.Id)) {
          valsBySensorId[v.sensor.Id] = new List<(double, Instant)>();
        }
        valsBySensorId[v.sensor.Id].Add((v.value, v.createdAt));
      }
      
      foreach (var (k, v) in valsBySensorId) {
        list.Add(new SensorDetailsApiModel(sensors[k], v));
      }

      return Ok(new SensorDetailsApiModelList(list));
    }

    [HttpGet("{deviceName}/{sensorName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<ActionResult<SensorDetailsApiModel>> Show(string deviceName, string sensorName, [FromQuery(Name = "started_at")] string? startedAtQuery = null, [FromQuery(Name = "duration")] string? durationQuery = null)
    {
      var device = await _service.GetDeviceByNameAsync(deviceName);
      var sensor = await _service.GetSensorByDeviceAndNameAsync(device, sensorName);   
      var values = new List<(double, Instant)>();

      _logger.LogTrace($"Getting query {startedAtQuery} and {durationQuery}");
      var duration = Duration.FromSeconds(3600);
      if (durationQuery != null) {
        try {
          var converted = TypeDescriptor.GetConverter(typeof(Duration)).ConvertFromString(durationQuery);
          if (converted != null) {
            duration = (Duration)converted;
          }
        } catch (NotSupportedException) {
          // do nothing
        } catch (UnparsableValueException exception) {
          throw new InvalidArgumentException(exception.Message, "duration");
        }
      }
      var startedAt = _clock.GetCurrentInstant()-duration;
      if (startedAtQuery != null) {
        try {
          var converted = TypeDescriptor.GetConverter(typeof(Instant)).ConvertFromString(startedAtQuery);
          if (converted != null) {
            startedAt = (Instant)converted;
          }
        } catch (NotSupportedException) {
          // do nothing
        } catch (UnparsableValueException exception) {
          throw new InvalidArgumentException(exception.Message, "started_at");
        }
      }
      _logger.LogTrace($"Querying from {startedAt} with {duration}");

      await foreach (var val in _valueService.GetNumberTimeSeriesBySensorAsync(sensor, startedAt, duration))
      {
        values.Add(val);
      }
      var res = new SensorDetailsApiModel(sensor, values);

      return Ok(res);
    }

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BasicSensorApiModel>> Create([BindRequired] SensorLocatorBindingModel view)
    {
      var device = await _service.GetDeviceByNameAsync(view.DeviceName);
      await _service.CreateSensorByDeviceAndNameAsync(device, view.Name);
      var sensor = await _service.GetSensorByDeviceAndNameAsync(device, view.Name);
      var res = new BasicSensorApiModel(sensor);
      return CreatedAtAction(nameof(Create), res);
    }
  }
}
