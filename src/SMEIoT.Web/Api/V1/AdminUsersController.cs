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
  [Authorize(Roles = "Admin")]
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
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<AdminUserApiModelList>> Index([FromQuery] int offset = 0, [FromQuery] int limit = 10, [FromQuery(Name="roles")] IEnumerable<string>? roles = null)
    {
      var list = new List<AdminUserApiModel>();
      await foreach (var (user, userRoles) in _userService.ListBasicUserResultAsync(offset, limit, roles))
      {
        list.Add(new AdminUserApiModel(user, userRoles));
      }

      return Ok(new AdminUserApiModelList(list, await _userService.NumberOfUsersAsync(roles)));
    }

    [HttpGet("{userName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminUserApiModel>> Show(string userName)
    {
      var model = await GetAdminUserResultAsync(userName);
      return Ok(model);
    }

    [HttpPut("{userName}/roles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<UserCredentialsUpdateApiModel>> EditRoles(
      [FromBody] UserRolesBindingModel binding, [FromRoute] string userName)
    {
      await _userService.UpdateUserRolesAsync(userName, binding.Roles);
      return Ok(await GetAdminUserResultAsync(userName));
    }

    private async Task<AdminUserApiModel> GetAdminUserResultAsync(string userName)
    {
      var (user, roles) = await _userService.GetUserAndRoleByName(userName);
      return new AdminUserApiModel(user, roles);
    }

  }
}
