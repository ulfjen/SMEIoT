using System;
using System.Threading.Tasks;
using SMEIoT.Core.Interfaces;
using SMEIoT.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SMEIoT.Core.Services
{
  public class DashboardService : IDashboardService
  {
    private readonly IApplicationDbContext _dbContext;
    private readonly UserManager<User> _userManager;

    public DashboardService(IApplicationDbContext dbContext, UserManager<User> userManager)
    {
      _dbContext = dbContext;
      _userManager = userManager;
    }

    public async Task<SystemHighlights> GetSystemHighlightsAsync()
    {
      var data = new SystemHighlights();
      data.UserCount = await _dbContext.Users.CountAsync();
      var admins = await _userManager.GetUsersInRoleAsync("Admin");
      data.AdminCount = admins.Count;
      data.ConnectedSensorCount = await _dbContext.Sensors.Where(s => s.Connected).CountAsync();
      data.SensorCount = await _dbContext.Sensors.CountAsync();
      data.ConnectedDeviceCount = await _dbContext.Devices.Where(s => s.Connected).CountAsync();
      data.DeviceCount = await _dbContext.Devices.CountAsync();
      return data;
    }
  }
}
