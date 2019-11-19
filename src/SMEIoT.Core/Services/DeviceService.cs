using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Exceptions;
using SMEIoT.Core.Interfaces;
using System;

namespace SMEIoT.Core.Services
{
  public class DeviceService : IDeviceService
  {
    private readonly IApplicationDbContext _dbContext;

    public DeviceService(IApplicationDbContext dbContext)
    {
      _dbContext = dbContext;
    } 
    
    public async Task<string> BootstrapDeviceWithPreSharedKeyAsync(string identity, string key)
    {
      return "";
    }

    public async Task<Device> GetDeviceByNameAsync(string deviceName)
    {
      throw new NotImplementedException();
    }
  }
}
