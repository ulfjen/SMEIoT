using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SMEIoT.Core.Interfaces;
using SMEIoT.Web.ApiModels;
using SMEIoT.Web.BindingModels;

namespace SMEIoT.Web.Api.V1
{
  [Route("api/admin/users")]
  public class AdminUsersController : BaseController
  {
    private readonly ILogger _logger;
    private readonly IUserManagementService _userService;

    public AdminUsersController(
      ILogger<UsersController> logger, IUserManagementService userService)
    {
      _logger = logger;
      _userService = userService;
    }

    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AdminUserApiModelList>> Index([FromQuery] int start = 1, [FromQuery] int limit = 10)
    {
      var list = new List<AdminUserApiModel>();
      await foreach (var (user, roles) in _userService.ListBasicUserResultAsync(start, limit))
      {
        list.Add(new AdminUserApiModel(user, roles));
      }

      return Ok(new AdminUserApiModelList {Users = list});
    }

    [HttpGet("{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AdminUserApiModel>> Show(string username)
    {
      var model = await GetAdminUserResultAsync(username);
      return Ok(model);
    }

    [HttpPut("{username}/roles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserCredentialsUpdateApiModel>> EditRoles(
      UserRolesBindingModel binding, string username)
    {
      await _userService.UpdateUserRoles(username, binding.Roles);
      return Ok(await GetAdminUserResultAsync(username));
    }

    private async Task<AdminUserApiModel> GetAdminUserResultAsync(string username)
    {
      var (user, roles) = await _userService.GetUserAndRoleByName(username);
      return new AdminUserApiModel(user, roles);
    }

  }
}
