using System.Threading.Tasks;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Exceptions;
using SMEIoT.Core.Services;
using SMEIoT.Infrastructure.Data;
using SMEIoT.Tests.Shared;
using Xunit;

namespace SMEIoT.Tests.Core.Services
{
  #if false
  public class SensorServiceTest
  {
    private static async Task<(SensorService, ApplicationDbContext)> BuildService()
    {
      var dbContext = ApplicationDbContextHelper.BuildTestDbContext();

      return (new SensorService(dbContext), dbContext);
    }

    [Fact]
    public async Task CreateSensor_ReturnsTrue()
    {
      // arrange
      var (service, dbContext) = await BuildService();
      
      // act
      var status = await service.CreateSensor("named-sensor");
      
      // assert
      Assert.True(status); 
    }
    
    [Fact]
    public async Task GetSensorByName_ThrowsIfNoSensor()
    {
      var (service, dbContext) = await BuildService();
      
      Task Act() => service.GetSensorByName("not-exist-sensor");

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("sensorName", notFound.ParamName);
    }
    
    [Fact]
    public async Task GetSensorByName_ReturnsSensor()
    {
      var (service, dbContext) = await BuildService();
      const string name = "named-sensor";
      dbContext.Sensors.Add(new Sensor{Name=name, NormalizedName = Sensor.NormalizeName(name)});
      await dbContext.SaveChangesAsync();

      var sensor = await service.GetSensorByName(name);
      
      Assert.Equal(name, sensor.Name); 
    }

  }
  #endif
}
