using System.Threading.Tasks;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Exceptions;
using SMEIoT.Core.Services;
using SMEIoT.Infrastructure.Data;
using SMEIoT.Tests.Shared;
using Xunit;

namespace SMEIoT.Tests.Core.Services
{
  public class DeviceServiceTest
  {
    private static async Task<(DeviceService, ApplicationDbContext)> BuildService()
    {
      var dbContext = ApplicationDbContextHelper.BuildTestDbContext();

      return (new DeviceService(dbContext), dbContext);
    }

    [Fact]
    public async Task BootstrapDeviceWithPreSharedKeyAsync_ReturnsPopulatedDevice()
    {
      // arrange
      var (service, dbContext) = await BuildService();
      
      // act
      var deviceName = await service.BootstrapDeviceWithPreSharedKeyAsync("identity", "key");
      
      // assert
      Assert.NotEmpty(deviceName);
    }

    [Fact]
    public async Task BootstrapDeviceWithPreSharedKeyAsync_PopulatesADevice()
    {
      var (service, dbContext) = await BuildService();
      
      var deviceName = await service.BootstrapDeviceWithPreSharedKeyAsync("Name", "key");
      var device = await service.GetDeviceByNameAsync(deviceName);
      
      Assert.Equal("Name", device.Name);
      Assert.NotEmpty(device.NormalizedName);
      Assert.Equal("key", device.PreSharedKey);
      Assert.Equal(DeviceAuthenticationType.PreSharedKey, device.AuthenticationType);
      Assert.False(device.Connected);
      Assert.Null(device.ConnectedAt);
      Assert.Null(device.LastMessageAt);
    }
    
    [Fact]
    public async Task GetDeviceByNameAsync_ThrowsIfNoDevice()
    {
      var (service, dbContext) = await BuildService();
      
      Task Act() => service.GetDeviceByNameAsync("not-exist-device");

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("deviceName", notFound.ParamName);
    }
    
    [Fact]
    public async Task GetDeviceByNameAsync_ReturnsDevice()
    {
      var (service, dbContext) = await BuildService();
      const string name = "named-device";
      dbContext.Devices.Add(new Device{Name=name, NormalizedName = Device.NormalizeName(name)});
      await dbContext.SaveChangesAsync();

      var device = await service.GetDeviceByNameAsync(name);
      
      Assert.Equal(name, device.Name); 
    }

    [Fact]
    public async Task GetDeviceByNameAsync_IgnoresCase()
    {
      var (service, dbContext) = await BuildService();
      const string name = "named-device";
      dbContext.Devices.Add(new Device{Name=name, NormalizedName = Device.NormalizeName(name)});
      await dbContext.SaveChangesAsync();

      var device = await service.GetDeviceByNameAsync("NAMED-device");
      
      Assert.Equal(name, device.Name); 
    }

  }
}
