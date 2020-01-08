using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using NodaTime;
using System;
using System.Linq;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;
using SMEIoT.Web.ApiModels;
using SMEIoT.Web.BindingModels;
using System.Diagnostics.Contracts; 

namespace SMEIoT.Web.Api.V1
{
  public class DevicesController : BaseController
  {
    private readonly ILogger _logger;
    private readonly IDeviceService _service;
    private readonly IDeviceSensorIdentifierSuggestService _identifierSuggestService;
    private readonly ISecureKeySuggestService _secureKeySuggestService;
    private readonly IMqttIdentifierService _mqttService;
    private const int DefaultDeviceNameSuggestWordLength = 2;

    public DevicesController(
      ILogger<DevicesController> logger,
      IDeviceService service,
      IDeviceSensorIdentifierSuggestService identifierSuggestService,
      IMqttIdentifierService mqttService,
      ISecureKeySuggestService secureKeySuggestService)
    {
      _logger = logger;
      _service = service;
      _mqttService = mqttService;
      _identifierSuggestService = identifierSuggestService;
      _secureKeySuggestService = secureKeySuggestService;
    }

    [HttpPost("bootstrap")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BasicDeviceApiModel>> BootstrapWithPreSharedKey([BindRequired] DeviceBootstrapConfigBindingModel view)
    {
      var deviceName = await _service.BootstrapDeviceWithPreSharedKeyAsync(view.Name, view.Key);
      var device = await _service.GetDeviceByNameAsync(deviceName);
      var res = new BasicDeviceApiModel(device);
      return CreatedAtAction(nameof(BootstrapWithPreSharedKey), res);
    }

    [HttpPut("{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BasicDeviceApiModel>> Update(string name, [BindRequired] DeviceConfigBindingModel key)
    {
      throw new NotImplementedException();
      var device = await _service.GetDeviceByNameAsync(name);
      var res = new BasicDeviceApiModel(device);
      return Ok(res);
    }

    [HttpGet("{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DeviceDetailsApiModel>> Show(string name)
    {
      var device = await _service.GetDeviceByNameAsync(name);

      var sensors = new List<BasicSensorApiModel>();
      await foreach (var sensor in _service.ListSensorsByDeviceAsync(device))
      {
        sensors.Add(new BasicSensorApiModel(sensor));
      }

      var sensorNamesFromMqtt = _mqttService.ListSensorNamesByDeviceName(device.Name);
      sensors.AddRange(sensorNamesFromMqtt.Select(n => new BasicSensorApiModel(n, device.Name)));

      var res = new DeviceDetailsApiModel(device, sensors);
      return Ok(res);
    }

    [HttpGet("{name}/basic")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BasicDeviceApiModel>> ShowBasic(string name)
    {
      var device = await _service.GetDeviceByNameAsync(name);
      var res = new BasicDeviceApiModel(device);
      return Ok(res);
    }

    [HttpGet("{name}/sensor_candidates")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<SensorCandidatesApiModel>> ListSensorCandidates(string name)
    {
      var suggestSensorNames = _mqttService.ListSensorNamesByDeviceName(name);
      return Ok(new SensorCandidatesApiModel(suggestSensorNames));
    }

    [HttpGet("config_suggest/bootstrap")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DeviceConfigSuggestApiModel>> SuggestBootstrap()
    {
      var device = await _service.GetARandomUnconnectedDeviceAsync();
      return Ok(new DeviceConfigSuggestApiModel(
        _identifierSuggestService.GenerateRandomIdentifierForDevice(DefaultDeviceNameSuggestWordLength),
        _secureKeySuggestService.GenerateSecureKeyWithByteLength(64),
        device?.Name
      ));
    }

    [HttpGet("config_suggest/key")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DeviceConfigSuggestApiModel>> SuggestSecureKey()
    {
      return Ok(new DeviceConfigSuggestApiModel(null, _secureKeySuggestService.GenerateSecureKeyWithByteLength(64))); // TODO: Add a max length
    }

    [HttpGet("config_suggest/device_name")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DeviceConfigSuggestApiModel>> SuggestDeviceName()
    {
      return Ok(new DeviceConfigSuggestApiModel(_identifierSuggestService.GenerateRandomIdentifierForDevice(DefaultDeviceNameSuggestWordLength)));
    }

    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BasicDeviceApiModelList>> Index([FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
      var list = new List<BasicDeviceApiModel>();
      await foreach (var device in _service.ListDevicesAsync(offset, limit))
      {
        list.Add(new BasicDeviceApiModel(device));
      }

      return Ok(new BasicDeviceApiModelList(list));
    }

  }
}
