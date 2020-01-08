using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Exceptions;
using SMEIoT.Core.Interfaces;

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
  }
}
