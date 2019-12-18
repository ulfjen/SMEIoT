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

    public DeviceService(IApplicationDbContext dbContext)
    {
      _dbContext = dbContext;
    } 
    
    public async Task<string> BootstrapDeviceWithPreSharedKeyAsync(string name, string key)
    {
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

    public async IAsyncEnumerable<Device> ListDevicesAsync(int start, int limit)
    {
      if (start <= 0)
      {
        throw new ArgumentException("start can't be negative or zero");
      }
      if (limit < 0)
      {
        throw new ArgumentException("limit can't be negative"); 
      }
      await foreach (var device in _dbContext.Devices.OrderBy(u => u.Id).Skip(start-1).Take(limit).AsAsyncEnumerable())
      {
        yield return device;
      }
    }

    public async IAsyncEnumerable<Sensor> ListSensorsAsync(int start, int limit)
    {
      if (start <= 0)
      {
        throw new ArgumentException("start can't be negative or zero");
      }
      if (limit < 0)
      {
        throw new ArgumentException("limit can't be negative"); 
      }
      await foreach (var sensor in _dbContext.Sensors.OrderBy(u => u.Id).Skip(start-1).Take(limit).AsAsyncEnumerable())
      {
        yield return sensor;
      }
    }

    public async Task CreateSensorByDeviceAndNameAsync(Device device, string sensorName)
    {
      _dbContext.Sensors.Add(new Sensor { Name = sensorName, NormalizedName = Sensor.NormalizeName(sensorName), DeviceId = device.Id });
      await _dbContext.SaveChangesAsync();
    }

    public async Task<Sensor> GetSensorByDeviceAndNameAsync(Device device, string sensorName)
    {
      var sensor = await _dbContext.Sensors.Where(s => s.NormalizedName == Sensor.NormalizeName(sensorName) && s.DeviceId == device.Id).FirstOrDefaultAsync();
      if (sensor == null)
      {
        throw new EntityNotFoundException("cannot find the sensor.", "sensorName");
      }

      return sensor;
    }

    public async IAsyncEnumerable<(double value, Instant createdAt)> GetNumberTimeSeriesByDeviceAndSensorAsync(Device device, Sensor sensor, Instant startedAt, Duration duration)
    {
      var query = from sv in _dbContext.SensorValues
                  where sv.SensorId == sensor.Id && sv.CreatedAt >= startedAt && sv.CreatedAt < startedAt + duration
                  select sv;
      foreach (var sv in query) {
        // throw new SystemException($"sv {sv.Value} {sv.CreatedAt}");
        yield return (sv.Value, sv.CreatedAt);
      }
    }
  }
}
