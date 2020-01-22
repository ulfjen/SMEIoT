using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using SMEIoT.Core.Interfaces;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SMEIoT.Core.Services
{
  /// we are relying on database to store values fast enough
  /// of course it can be improved by batche. Now we relies on PG's parameters and our runloop interval
  public class MqttMessageIngestionService : IMqttMessageIngestionService
  {
    public const string SensorTopicPrefix = "iot/";
    public const string BrokerTopicPrefix = "$SYS/broker/";
    private const string SecondsPostfix = "seconds";

    private readonly IDeviceService _deviceService;
    private readonly IMqttIdentifierService _mqttIdentifierService;
    private readonly ISensorValueService _valueService;
    private readonly IMosquittoBrokerService _brokerService;
    private readonly ILogger _logger;

    public MqttMessageIngestionService(
      IDeviceService deviceService,
      IMqttIdentifierService mqttIdentifierService,
      IMosquittoBrokerService brokerService,
      ISensorValueService valueService,
      ILogger<MqttMessageIngestionService> logger
    )
    {
      _deviceService = deviceService;
      _mqttIdentifierService = mqttIdentifierService;
      _brokerService = brokerService;
      _valueService = valueService;
      _logger = logger;
    }

    public async Task ProcessCommonMessageAsync(MqttMessage message)
    {
      if (!message.Topic.StartsWith(SensorTopicPrefix)) { return; }

      var parsed = message.Topic.Substring(SensorTopicPrefix.Length);
      var splitP = parsed.IndexOf('/');

      if (splitP == -1) {
        return;
      }


      string deviceName = String.Empty;
      string sensorName = String.Empty;

      try {
        deviceName = parsed.Substring(0, splitP);
        _logger.LogTrace($"deviceName {deviceName}");
        await _mqttIdentifierService.RegisterDeviceNameAsync(deviceName);

        sensorName = parsed.Substring(splitP+1);
        _logger.LogTrace($"sensorName {sensorName}");
        await _mqttIdentifierService.RegisterSensorNameWithDeviceNameAsync(sensorName, deviceName);
      }
      catch (InvalidArgumentException exception) {
        _logger.LogWarning($"We can't parse the device or sensor name presented in message {message.Topic}. {exception.Message}");
      }
    
      Device device = null!;
      bool sensorFound = false;
      Sensor sensor = null!;

      try {
        device = await _deviceService.GetDeviceByNameAsync(deviceName);
      } catch (EntityNotFoundException exception) {
        _logger.LogError($"We can't find the device in message {message.Topic}. {exception.Message}");
        return;
      }

      try {
        sensor = await _deviceService.GetSensorByDeviceAndNameAsync(device, sensorName);
        sensorFound = true;
      } catch (EntityNotFoundException exception) {
        _logger.LogError($"We can't find the sensor in message {message.Topic}. {exception.Message}");
      }

      if (sensorFound) {
        await _deviceService.UpdateSensorAndDeviceTimestampsAndStatusAsync(sensor, message.ReceivedAt);
      } else {
        await _deviceService.UpdateDeviceTimestampsAndStatusAsync(device, message.ReceivedAt);
        return;
      }

      _logger.LogTrace($"recording {message.Payload}");
      if (Double.TryParse(message.Payload, out var doubleVal)) {
        await _valueService.AddSensorValueAsync(sensor, doubleVal, message.ReceivedAt);
      } else {
        _logger.LogError($"We can't parse the message as a double. {message.Topic} {message.Payload}");
      }
    }

    public async Task ProcessBrokerMessageAsync(MqttMessage message)
    {
      if (!message.Topic.StartsWith(BrokerTopicPrefix)) { return; }

      var parsed = message.Topic.Substring(BrokerTopicPrefix.Length);
      var value = message.Payload;

      if (value.EndsWith(SecondsPostfix)) {
        value = value.Substring(0, value.Length - SecondsPostfix.Length).TrimEnd();
      }
      var res = await _brokerService.RegisterBrokerStatisticsAsync(parsed.ToString(), value.ToString(), message.ReceivedAt);
      _logger.LogTrace($"processing broker message: {parsed.ToString()} = {value.ToString()} {message.ReceivedAt} {res}");
    }
  }
}
