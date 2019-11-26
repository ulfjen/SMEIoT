using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NodaTime;
using System;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;
using SMEIoT.Web.ApiModels;
using SMEIoT.Web.BindingModels;

namespace SMEIoT.Web.Api.V1
{
  public class DevicesController : BaseController
  {
    private readonly ILogger<DevicesController> _logger;
    private readonly IDeviceService _service;
    private readonly IDeviceSensorIdentifierSuggestService _identifierSuggestService;
    private readonly ISecureKeySuggestService _secureKeySuggestService;

    public DevicesController(
      ILogger<DevicesController> logger,
      IDeviceService service,
      IDeviceSensorIdentifierSuggestService identifierSuggestService,
      ISecureKeySuggestService secureKeySuggestService)
    {
      _logger = logger;
      _service = service;
      _identifierSuggestService = identifierSuggestService;
      _secureKeySuggestService = secureKeySuggestService;
    }

    [HttpPost("bootstrap")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DeviceApiModel>> BootstrapWithPreSharedKey(DeviceBootstrapConfigBindingModel view)
    {
      var deviceName = await _service.BootstrapDeviceWithPreSharedKeyAsync(view.Identity, view.Key);
      var device = await _service.GetDeviceByNameAsync(deviceName);
      var res = new DeviceApiModel(device);
      return CreatedAtAction(nameof(BootstrapWithPreSharedKey), res);
    }

    [HttpPut("{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DeviceApiModel>> Update(DeviceConfigBindingModel view)
    {
      var device = await _service.GetDeviceByNameAsync(view.Name);
      var res = new DeviceApiModel(device);
      return Ok(res);
    }

    [HttpGet("sensor_candidates")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<SensorCandidatesApiModel>> ListSensorCandidates()
    {
      throw new NotImplementedException();
      // var sensor = _mqttService.ListSensorNames("dummy");
      return Ok(new SensorCandidatesApiModel(new[]{"name1", "name2"}));
    }

    [HttpGet("suggest_bootstrap_config")]
    public async Task<ActionResult<DeviceConfigSuggestApiModel>> SuggestBootstrap()
    {
      var device = await _service.GetARandomUnconnectedDeviceAsync();
      return Ok(new DeviceConfigSuggestApiModel(_identifierSuggestService.GenerateRandomIdentifierForDevice(2), _secureKeySuggestService.GenerateSecureKey(64), device?.Name));
    }

    [HttpGet("suggest_key")]
    public async Task<ActionResult<DeviceConfigSuggestApiModel>> SuggestSecureKey()
    {
      return Ok(new DeviceConfigSuggestApiModel(null, _secureKeySuggestService.GenerateSecureKey(64)));
    }

    [HttpGet("suggest_device_name")]
    public async Task<ActionResult<DeviceConfigSuggestApiModel>> SuggestDeviceName()
    {
      return Ok(new DeviceConfigSuggestApiModel(_identifierSuggestService.GenerateRandomIdentifierForDevice(2)));
    }
  }
}
