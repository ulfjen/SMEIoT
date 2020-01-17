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

      _logger.LogTrace($"{parsed.Substring(0, splitP)}:{parsed.Substring(splitP+1)}");

      string deviceName = String.Empty;
      string sensorName = String.Empty;

      try {
        deviceName = parsed.Substring(0, splitP);
        await _mqttIdentifierService.RegisterDeviceNameAsync(deviceName);

        sensorName = parsed.Substring(splitP+1);
        await _mqttIdentifierService.RegisterSensorNameWithDeviceNameAsync(sensorName, deviceName);
      }
      catch (InvalidArgumentException exception) {
        _logger.LogError($"We can't parse the device or sensor name presented in message {message.Topic}. {exception.Message}");
        return;
      }
    
      Device device = null!;
      Sensor sensor = null!;

      try {
        device = await _deviceService.GetDeviceByNameAsync(deviceName);
        sensor = await _deviceService.GetSensorByDeviceAndNameAsync(device, sensorName);
      } catch (EntityNotFoundException exception) {
        _logger.LogError($"We can't find the device or sensor in message {message.Topic}. {exception.Message}");
        return;
      }

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
