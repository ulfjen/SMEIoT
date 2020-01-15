using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Exceptions;
using SMEIoT.Core.Interfaces;
using NodaTime;

namespace SMEIoT.Core.Services
{
  public class DeviceService : IDeviceService
  {
    private readonly IApplicationDbContext _dbContext;
    private readonly IMqttIdentifierService _identifierService;
    public static readonly List<string> ForbiddenDeviceNames = new List<string> { "config_suggest", "bootstrap", "wait_connection", "configure_sensors", "new" }; 

    public DeviceService(IApplicationDbContext dbContext, IMqttIdentifierService identifierService)
    {
      _dbContext = dbContext;
      _identifierService = identifierService;
    } 
    
    public async Task<string> BootstrapDeviceWithPreSharedKeyAsync(string name, string key)
    {
      if (ForbiddenDeviceNames.Contains(name)) {
        throw new InvalidArgumentException($"Reserverd device name {name}.", "name");
      }
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
      if (offset < 0)
      {
        throw new InvalidArgumentException("Offset can't be negative.", "offset");
      }
      if (limit < 0)
      {
        throw new InvalidArgumentException("Limit can't be negative.", "limit"); 
      }
      await foreach (var device in _dbContext.Devices.OrderBy(u => u.Id).Skip(offset).Take(limit).AsAsyncEnumerable())
      {
        yield return device;
      }
    }

    public async IAsyncEnumerable<Sensor> ListSensorsAsync(int offset, int limit)
    {
      if (offset < 0)
      {
        throw new InvalidArgumentException("Offset can't be negative.", "offset");
      }
      if (limit < 0)
      {
        throw new InvalidArgumentException("Limit can't be negative.", "limit");       
      }
      await foreach (var sensor in _dbContext.Sensors.OrderBy(u => u.Id).Skip(offset).Take(limit).AsAsyncEnumerable())
      {
        yield return sensor;
      }
    }

    public async Task CreateSensorByDeviceAndNameAsync(Device device, string sensorName)
    {
      var legalNames = await _identifierService.ListSensorNamesByDeviceNameAsync(device.Name);
      if (!legalNames.Contains(sensorName)) {
        throw new EntityNotFoundException($"We can't find messages from sensor {device.Name}/{sensorName}.", nameof(sensorName));
      }
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
  }
}
