using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SMEIoT.Core.Entities;
using SMEIoT.Tests.Shared;
using SMEIoT.Web.Api.V1;
using Xunit;

namespace SMEIoT.Tests.Api.V1
{
  public class UsersControllerTest
  {
    #if false
    static UsersController BuildDefaultUserController()
    {
      var userManager = MockHelpers.CreateUserManager();
            var controller = new UsersController(userManager, MockHelpers.CreateRoleManager(),
              new NullLogger<UsersController>());
            return controller;
    }
    
    [Fact]
    public async void Create_ReturnsBasicUserView()
    {
      // arrange
      var controller = BuildDefaultUserController();

      // act
      var result =
        await controller.Create(
          new ValidatedUserCredentialsViewModel {Username = "admin", Password = "dummy-password-1"});

      // assert
      var actionResult = Assert.IsType<ActionResult<BasicUserViewModel>>(result);
      var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
      var returnValue = Assert.IsType<BasicUserViewModel>(createdAtActionResult.Value);
      Assert.Equal("admin", returnValue.Username);
      Assert.Equal("ADMIN", returnValue.Roles.FirstOrDefault());
    }

    [Fact]
    public async void Create_AssignsRoleAdminToFirstUser()
    {
      var controller = BuildDefaultUserController();
      await controller.Create(
        new ValidatedUserCredentialsViewModel {Username = "user1", Password = "dummy-password-1"});
      
      var result =
        await controller.Create(
          new ValidatedUserCredentialsViewModel {Username = "user2", Password = "dummy-password-1"});

      var actionResult = Assert.IsType<ActionResult<BasicUserViewModel>>(result);
      var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
      var returnValue = Assert.IsType<BasicUserViewModel>(createdAtActionResult.Value);
      Assert.Equal("user2", returnValue.Username);
      Assert.Null(returnValue.Roles.FirstOrDefault());
    }
    
    [Fact]
    public async void Show_ReturnsABasicView()
    {
      var userManager = MockHelpers.CreateUserManager();
      var controller = new UsersController(userManager, MockHelpers.CreateRoleManager(),
        new NullLogger<UsersController>());
      await userManager.CreateAsync(new User {UserName = "user1"}, "password1");
      await userManager.CreateAsync(new User {UserName = "user2"}, "password2");
      await userManager.CreateAsync(new User {UserName = "user3"}, "password3");

      var result = await controller.Show("user2");

      var actionResult = Assert.IsType<ActionResult<BasicUserViewModel>>(result);
      var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
      var returnValue = Assert.IsType<BasicUserViewModel>(okObjectResult.Value);
      Assert.Equal("user2", returnValue.Username);
      Assert.Null(returnValue.Roles.FirstOrDefault());
    }
    
    [Fact]
    public async void Show_ReturnsNotFound()
    {
      var userManager = MockHelpers.CreateUserManager();
      var controller = new UsersController(userManager, MockHelpers.CreateRoleManager(),
        new NullLogger<UsersController>());
      await userManager.CreateAsync(new User {UserName = "user1"}, "password1");

      var result = await controller.Show("user2");

      var actionResult = Assert.IsType<ActionResult<BasicUserViewModel>>(result);
      var notFoundResult = Assert.IsType<NotFoundResult>(actionResult.Result);
      Assert.Equal(404, notFoundResult.StatusCode);
    }
    
    [Fact]
    public async void Edit_ChangesPassword()
    {
      var userManager = MockHelpers.CreateUserManager();
      var controller = new UsersController(userManager, MockHelpers.CreateRoleManager(),
        new NullLogger<UsersController>());
      await userManager.CreateAsync(new User {UserName = "user1"}, "password1");

      var result = await controller.Edit(new ManageUserViewModel{Password = "password1", ChangedPassword = "new-password"}, "user1");

      var actionResult = Assert.IsType<ActionResult<ManageUserResultViewModel>>(result);
      var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
      var returnValue = Assert.IsType<ManageUserResultViewModel>(okObjectResult.Value);
      Assert.Equal("user1", returnValue.Username);
    }
    
    [Fact]
    public async void Edit_ReturnsMismatchChangesPassword()
    {
      var userManager = MockHelpers.CreateUserManager();
      var controller = new UsersController(userManager, MockHelpers.CreateRoleManager(),
        new NullLogger<UsersController>());
      await userManager.CreateAsync(new User {UserName = "user1"}, "password1");

      var result = await controller.Edit(new ManageUserViewModel{Password = "wrong-password", ChangedPassword = "new-password"}, "user1");

      var actionResult = Assert.IsType<ActionResult<ManageUserResultViewModel>>(result);
      var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }
#endif
  }
}
