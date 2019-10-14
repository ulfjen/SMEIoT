using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NodaTime;
using SMEIoT.Core.Entities;
using SMEIoT.Infrastructure.Data;
using Xunit;

namespace SMEIoT.Tests.Controllers.Api.V1
{
  public class SensorsControllerTest
  {
    #if false
    static SensorsController BuildDefaultSensorController(bool mock)
    {
      var mockDb = new Mock<ApplicationDbContext>();
      mockDb.Setup(ctx => ctx.Where(It.IsAny<BrainstormSession>()))
        .Returns(Task.CompletedTask)
        .Verifiable(); 
      var controller = new SensorsController(new NullLogger<SensorsController>(), new ApplicationDbContext(TestDbContextOptions.GetOptions(), SystemClock.Instance));
      return controller;
    }
    
    [Fact]
    public async void Create_ReturnsASensor()
    {
      // arrange
      var controller = BuildDefaultSensorController();

      // act
      var result =
        await controller.Create(new BasicSensorViewModel{Name="a-normal-sensor"});

      // assert
      var actionResult = Assert.IsType<ActionResult<Sensor>>(result);
      var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
      var returnValue = Assert.IsType<Sensor>(createdAtActionResult.Value);
      Assert.Equal("a-normal-sensor", returnValue.Name);
      Assert.Equal("A-NORMAL-SENSOR", returnValue.NormalizedName);
    }

        
    [Fact]
    public async void Assign_ReturnsAssignmentView()
    {
      // arrange
      var controller = BuildDefaultSensorController();

      // act
      var result =
        await controller.Assign(new AssignUserSensorViewModel{Name="a-normal-sensor", Username = "normal-user-1"});

      // assert
      var actionResult = Assert.IsType<ActionResult<SensorAssignment>>(result);
      var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
      var returnValue = Assert.IsType<SensorAssignment>(createdAtActionResult.Value);
      Assert.Equal("a-normal-sensor", returnValue.Name);
      Assert.Equal("normal-user-1", returnValue.Username);
      Assert.Equal(1, returnValue.Assignments);
    }
    
    [Fact]
    public async void Assign_ReturnsErrorsIfAny()
    {
      // arrange
      var controller = BuildDefaultSensorController();

      // act
      var result =
        await controller.Assign(new AssignUserSensorViewModel{Name="a-normal-sensor", Username = "non-existed-user"});

      // assert
      var actionResult = Assert.IsType<ActionResult<SensorAssignment>>(result);
      var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
      var returnValue = Assert.IsType<Dictionary<string, string>>(notFoundResult.Value);
      Assert.Equal("username", "can not find the user.");
    }
    
    [Fact]
    public async void Assign_ReturnsErrorsIfNoSensor()
    {
      // arrange
      var controller = BuildDefaultSensorController();

      // act
      var result =
        await controller.Assign(new AssignUserSensorViewModel{Name="not-existed-sensor", Username = "normal-user-1"});

      // assert
      var actionResult = Assert.IsType<ActionResult<SensorAssignment>>(result);
      var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
      var returnValue = Assert.IsType<Dictionary<string, string>>(notFoundResult.Value);
      Assert.Equal("name", "can not find the sensor.");
    }
    
    [Fact]
    public async void RevokeAssignment_ReturnsErrorsIfAny()
    {
      // arrange
      var controller = BuildDefaultSensorController();

      // act
      var result =
        await controller.Assign(new AssignUserSensorViewModel{Name="a-normal-sensor", Username = "non-existed-user"});

      // assert
      var actionResult = Assert.IsType<ActionResult<SensorAssignment>>(result);
      var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
      var returnValue = Assert.IsType<Dictionary<string, string>>(notFoundResult.Value);
      Assert.Equal("username", "can not find the user.");
    }
    
    [Fact]
    public async void RevokeAssignment_ReturnsErrorsIfNoSensor()
    {
      // arrange
      var controller = BuildDefaultSensorController();

      // act
      var result =
        await controller.Assign(new AssignUserSensorViewModel{Name="not-existed-sensor", Username = "normal-user-1"});

      // assert
      var actionResult = Assert.IsType<ActionResult<SensorAssignment>>(result);
      var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
      var returnValue = Assert.IsType<Dictionary<string, string>>(notFoundResult.Value);
      Assert.Equal("name", "can not find the sensor.");
    }
            
    [Fact]
    public async void RevokeAssignment_ReturnsAssignmentView()
    {
      // arrange
      var controller = BuildDefaultSensorController();

      // act
      var result =
        await controller.RevokeAssignment(new AssignUserSensorViewModel{Name="a-normal-sensor", Username = "normal-user-1"});

      // assert
      var actionResult = Assert.IsType<ActionResult<Sensor>>(result);
      var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
      var returnValue = Assert.IsType<SensorAssignment>(createdAtActionResult.Value);
      Assert.Equal("a-normal-sensor", returnValue.Name);
      Assert.Equal("normal-user-1", returnValue.Username);
      Assert.Equal(0, returnValue.Assignments);
    }
#endif
  }
}
