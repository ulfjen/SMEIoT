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
  [Authorize(Roles = "Admin")]
  public class DevicesController : BaseController
  {
    private readonly ILogger _logger;
    private readonly IDeviceService _service;
    private readonly IMqttEntityIdentifierSuggestionService _identifierSuggestService;
    private readonly ISecureKeySuggestionService _secureKeySuggestionService;
    private readonly IMqttIdentifierService _mqttService;
    private readonly IMqttClientConfigService _configService;
    private readonly IServerHostAccessor _hostAccessor;
    private readonly ISensorValueService _valueService;

    private const int DefaultDeviceNameSuggestWordLength = 2;
    private const int DefaultKeyByteLength = 64;
    private const bool ShowRealHostOrIp = true;

    public DevicesController(
      ILogger<DevicesController> logger,
      IDeviceService service,
      IMqttEntityIdentifierSuggestionService identifierSuggestService,
      IMqttIdentifierService mqttService,
      ISecureKeySuggestionService SecureKeySuggestionService,
      IMqttClientConfigService configService,
      IServerHostAccessor hostAccessor,
      ISensorValueService valueService)
    {
      _logger = logger;
      _service = service;
      _mqttService = mqttService;
      _identifierSuggestService = identifierSuggestService;
      _secureKeySuggestionService = SecureKeySuggestionService;
      _configService = configService;
      _hostAccessor = hostAccessor;
      _valueService = valueService;
    }

    private async Task<MqttBrokerConnectionInformation> GetBrokerConnectionInfoAsync()
    {
      var info = await _configService.SuggestConfigAsync();
      var realHost = await _hostAccessor.GetServerHostAsync();
      if (ShowRealHostOrIp && realHost != null) {
        info.Host = realHost;
      }
      return info;
    }

    [HttpPost("bootstrap")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<BasicDeviceApiModel>> BootstrapWithPreSharedKey([BindRequired] DeviceBootstrapConfigBindingModel view)
    {
      var deviceName = await _service.BootstrapDeviceWithPreSharedKeyAsync(view.Name, view.Key);
      var device = await _service.GetDeviceByNameAsync(deviceName);
      var info = await GetBrokerConnectionInfoAsync();
      var res = new BasicDeviceApiModel(device, info);
      return CreatedAtAction(nameof(BootstrapWithPreSharedKey), res);
    }

    [HttpPut("{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<BasicDeviceApiModel>> Update(string name, [BindRequired] DeviceConfigBindingModel config)
    {
      var device = await _service.GetDeviceByNameAsync(name);
      await _service.UpdateDeviceConfigAsync(device, config.Key);
      device = await _service.GetDeviceByNameAsync(name);
      var info = await GetBrokerConnectionInfoAsync();
      var res = new BasicDeviceApiModel(device, info);
      return Ok(res);
    }

    [HttpGet("{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<DeviceDetailsApiModel>> Show(string name)
    {
      var device = await _service.GetDeviceByNameAsync(name);

      var sensors = new List<Sensor>();
      var valsBySensorId = new Dictionary<long, List<(double, Instant)>>();
      await foreach (var sensor in _service.ListSensorsByDeviceAsync(device))
      {
        sensors.Add(sensor);
        valsBySensorId[sensor.Id] = new List<(double, Instant)>();
      }
      await foreach (var (sensor, value, createdAt) in _valueService.GetLastNumberOfValuesBySensorsAsync(sensors, 10))
      {
        valsBySensorId[sensor.Id].Add((value, createdAt));
      }
      
      var sensorList = new List<SensorDetailsApiModel>();
      sensorList.AddRange(sensors.Select(s => new SensorDetailsApiModel(s, valsBySensorId[s.Id])));

      var sensorNamesFromMqtt = await _mqttService.ListSensorNamesByDeviceNameAsync(device.Name);
      var registeredSensorNames = sensors.Select(s => s.Name);
      sensorList.AddRange(sensorNamesFromMqtt.Except(registeredSensorNames).Select(n => new SensorDetailsApiModel(n, device.Name)));

      var info = await GetBrokerConnectionInfoAsync();
      var res = new DeviceDetailsApiModel(device, sensorList, info);
      return Ok(res);
    }

    [HttpGet("{name}/basic")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BasicDeviceApiModel>> ShowBasic(string name)
    {
      var device = await _service.GetDeviceByNameAsync(name);
      var info = await GetBrokerConnectionInfoAsync();
      var res = new BasicDeviceApiModel(device, info);
      return Ok(res);
    }

    [HttpGet("{name}/sensor_candidates")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SensorCandidatesApiModel>> ListSensorCandidates(string name)
    {
      var suggestSensorNames = await _mqttService.ListSensorNamesByDeviceNameAsync(name);
      return Ok(new SensorCandidatesApiModel(suggestSensorNames));
    }

    [HttpGet("config_suggest/bootstrap")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DeviceConfigSuggestApiModel>> SuggestBootstrap()
    {
      var device = await _service.GetARandomUnconnectedDeviceAsync();
      return Ok(new DeviceConfigSuggestApiModel(
        await _identifierSuggestService.GenerateRandomIdentifierForDeviceAsync(DefaultDeviceNameSuggestWordLength),
        await _secureKeySuggestionService.GenerateSecureKeyWithByteLengthAsync(DefaultKeyByteLength),
        device?.Name
      ));
    }

    [HttpGet("config_suggest/key")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DeviceConfigSuggestApiModel>> SuggestSecureKey()
    {
      return Ok(new DeviceConfigSuggestApiModel(
        null,
        await _secureKeySuggestionService.GenerateSecureKeyWithByteLengthAsync(DefaultKeyByteLength)
      ));
    }

    [HttpGet("config_suggest/device_name")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DeviceConfigSuggestApiModel>> SuggestDeviceName()
    {
      return Ok(new DeviceConfigSuggestApiModel(
        await _identifierSuggestService.GenerateRandomIdentifierForDeviceAsync(DefaultDeviceNameSuggestWordLength)
      ));
    }

    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<BasicDeviceApiModelList>> Index([FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
      var list = new List<BasicDeviceApiModel>();
      var info = await GetBrokerConnectionInfoAsync();
      await foreach (var device in _service.ListDevicesAsync(offset, limit))
      {
        list.Add(new BasicDeviceApiModel(device, info));
      }

      return Ok(new BasicDeviceApiModelList(list));
    }

    [HttpDelete("{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task Delete(string name)
    {
      await _service.RemoveDeviceByNameAsync(name);
    }


    [HttpDelete("{deviceName}/{sensorName}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task DeleteSensor(string deviceName, string sensorName)
    {
      var device = await _service.GetDeviceByNameAsync(deviceName);
      await _service.RemoveSensorByDeviceAndNameAsync(device, sensorName);
    }
  }
}
