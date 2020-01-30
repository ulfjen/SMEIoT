using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Exceptions;
using SMEIoT.Core.Interfaces;
using SMEIoT.Core.Helpers;
using NodaTime;

namespace SMEIoT.Core.Services
{
  public class DeviceService : IDeviceService
  {
    private readonly IApplicationDbContext _dbContext;
    private readonly IMqttIdentifierService _identifierService;
    public static readonly List<string> ForbiddenDeviceNames = new List<string> { "config_suggest", "bootstrap", "wait_connection", "configure_sensors", "new", "broker" }; 
    public const int LastMessageAtTimestampUpdateGracePeriod = 40;

    public DeviceService(IApplicationDbContext dbContext, IMqttIdentifierService identifierService)
    {
      _dbContext = dbContext;
      _identifierService = identifierService;
    } 
    
    private Task ValidateDeviceNameAsync(string name)
    {
      if (ForbiddenDeviceNames.Contains(name)) {
        throw new InvalidArgumentException($"Reserverd device name {name}.", nameof(name));
      }
      if (name.Length > 1000) {
        throw new InvalidArgumentException($"Device name can't be longer than 1000.", nameof(name));
      }
      if (name.Length < 3) {
        throw new InvalidArgumentException($"Device name can't be shorter than 3.", nameof(name));
      }
      return Task.CompletedTask;
    }

    private static HashSet<char> AllowedCharsInKey = new HashSet<char>("0123456789ABCDEF");

    private Task ValidateDeviceKeyAsync(string key)
    {
      if (key.Length > SecureKeySuggestionService.ByteLengthUpperBound*2) {
        throw new InvalidArgumentException($"Device key can't be longer than {SecureKeySuggestionService.ByteLengthUpperBound*2}.", nameof(key));
      }
      if (key.Length < SecureKeySuggestionService.ByteLengthLowerBound*2) {
        throw new InvalidArgumentException($"Device key can't be shorter than {SecureKeySuggestionService.ByteLengthLowerBound*2}.", nameof(key));
      }
      var chars = new HashSet<char>(key.ToUpperInvariant());
      if (!chars.IsSubsetOf(AllowedCharsInKey)) {
        throw new InvalidArgumentException($"Device key must be hex value.", nameof(key));
      }
      return Task.CompletedTask;
    }

    public async Task<string> BootstrapDeviceWithPreSharedKeyAsync(string name, string key)
    {
      await ValidateDeviceNameAsync(name);
      await ValidateDeviceKeyAsync(key);

      _dbContext.Devices.Add(new Device
        {
          Name = name,
          NormalizedName = Device.NormalizeName(name),
          AuthenticationType = DeviceAuthenticationType.PreSharedKey,
          PreSharedKey = key
      });
      await _dbContext.SaveChangesAsync();
      return name;
    }

    public async Task<Device> GetDeviceByNameAsync(string deviceName)
    {      
      var device = await _dbContext.Devices.Where(d => d.NormalizedName == Device.NormalizeName(deviceName)).FirstOrDefaultAsync();
      if (device == null)
      {
        throw new EntityNotFoundException("cannot find the device.", nameof(deviceName));
      }

      return device;
    }

    public async Task<Device?> GetARandomUnconnectedDeviceAsync()
    {
      return await (from d in _dbContext.Devices
                    where !d.Connected
                    orderby Guid.NewGuid()
                    select d).FirstOrDefaultAsync();
    }

    public async IAsyncEnumerable<Device> ListDevicesAsync(int offset, int limit)
    {
      RangeQueryValidations.ValidateRangeQueryParameters(offset, limit);
      await foreach (var device in _dbContext.Devices.OrderBy(u => u.Id).Skip(offset).Take(limit).AsAsyncEnumerable())
      {
        yield return device;
      }
    }

    public async Task UpdateDeviceConfigAsync(Device device, string key)
    {
      await ValidateDeviceKeyAsync(key);
      device.PreSharedKey = key;
      _dbContext.Devices.Update(device);
      await _dbContext.SaveChangesAsync();
    } 

    public async IAsyncEnumerable<Sensor> ListSensorsAsync(int offset, int limit)
    {
      RangeQueryValidations.ValidateRangeQueryParameters(offset, limit);

      await foreach (var sensor in _dbContext.Sensors.Include(s => s.Device).OrderBy(u => u.Id).Skip(offset).Take(limit).AsAsyncEnumerable())
      {
        yield return sensor;
      }
    }

    public async Task<int> NumberOfSensorsAsync()
    {
      return await _dbContext.Sensors.CountAsync();
    }

    public async Task CreateSensorByDeviceAndNameAsync(Device device, string sensorName)
    {
      var legalNames = await _identifierService.ListSensorNamesByDeviceNameAsync(device.Name);
      if (!legalNames.Contains(sensorName)) {
        throw new EntityNotFoundException($"We can't find messages from sensor {device.Name}/{sensorName}.", nameof(sensorName));
      }
      var found = await _dbContext.Sensors.Where(s => s.NormalizedName == Sensor.NormalizeName(sensorName) && s.DeviceId == device.Id).CountAsync();
      if (found > 0) { throw new EntityExistException($"The sensor {sensorName} was created. You can't create a new one.", nameof(sensorName)); }
      _dbContext.Sensors.Add(new Sensor { Name = sensorName, NormalizedName = Sensor.NormalizeName(sensorName), DeviceId = device.Id });
      await _dbContext.SaveChangesAsync();
    }


    public async IAsyncEnumerable<Sensor> ListSensorsByDeviceAsync(Device device)
    {
      await foreach (var sensor in _dbContext.Sensors.Where(s => s.DeviceId == device.Id).AsAsyncEnumerable())
      {
        yield return sensor;
      }
    }

    public async Task<Sensor> GetSensorByDeviceAndNameAsync(Device device, string sensorName)
    {
      var sensor = await _dbContext.Sensors.Where(s => s.NormalizedName == Sensor.NormalizeName(sensorName) && s.DeviceId == device.Id).FirstOrDefaultAsync();
      if (sensor == null)
      {
        throw new EntityNotFoundException($"Cannot find the sensor {sensorName}.", nameof(sensorName));
      }

      return sensor;
    }

    public async IAsyncEnumerable<(double value, Instant createdAt)> GetNumberTimeSeriesByDeviceAndSensorAsync(Sensor sensor, Instant startedAt, Duration duration)
    {
      var query = from sv in _dbContext.SensorValues
                  where sv.SensorId == sensor.Id && sv.CreatedAt >= startedAt && sv.CreatedAt < startedAt + duration
                  select sv;
      await foreach (var sv in query.AsAsyncEnumerable()) {
        yield return (sv.Value, sv.CreatedAt);
      }
    }

    public async Task RemoveSensorByDeviceAndNameAsync(Device device, string sensorName)
    {
      var sensor = await _dbContext.Sensors.FirstOrDefaultAsync(s => s.NormalizedName == Sensor.NormalizeName(sensorName));
      if (sensor == null) {
        throw new EntityNotFoundException($"Cannot find the sensor {sensorName}.", nameof(sensorName));
      }
      _dbContext.Sensors.Remove(sensor);
      await _dbContext.SaveChangesAsync();
    }
    
    public async Task RemoveDeviceByNameAsync(string deviceName)
    {
      var device = await _dbContext.Devices.FirstOrDefaultAsync(s => s.NormalizedName == Device.NormalizeName(deviceName));
      if (device == null) {
        throw new EntityNotFoundException($"Cannot find the device {deviceName}.", nameof(deviceName));
      }
      _dbContext.Devices.Remove(device);
      await _dbContext.SaveChangesAsync();
    }

    private bool UpdateMqttEntityTimestampsAndStatus(MqttEntityBase entity, Instant receivedAt)
    {
      var updated = false;
      if (entity.Connected != true) {
        entity.Connected = true;
        updated = true;
      }
      if (entity.ConnectedAt == null) {
        entity.ConnectedAt = receivedAt;
        updated = true;
      }

      if (entity.LastMessageAt == null || entity.LastMessageAt + Duration.FromSeconds(LastMessageAtTimestampUpdateGracePeriod) <= receivedAt) {
        entity.LastMessageAt = receivedAt;
        updated = true;
      }
      return updated;
    }

    public async Task UpdateDeviceTimestampsAndStatusAsync(Device device, Instant receivedAt)
    {
      if (UpdateMqttEntityTimestampsAndStatus(device, receivedAt)) {
        _dbContext.Devices.Update(device);
        await _dbContext.SaveChangesAsync();
      }
    }

    public async Task UpdateSensorAndDeviceTimestampsAndStatusAsync(Sensor sensor, Instant receivedAt)
    {
      if (UpdateMqttEntityTimestampsAndStatus(sensor, receivedAt)) {
        UpdateMqttEntityTimestampsAndStatus(sensor.Device, receivedAt);
        // Device is auto tracked.
        _dbContext.Sensors.Update(sensor);
        await _dbContext.SaveChangesAsync();
      }
    }
  }
}
