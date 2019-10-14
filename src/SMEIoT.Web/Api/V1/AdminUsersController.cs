using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SMEIoT.Core.Interfaces;
using SMEIoT.Web.ApiModels;

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
  }
}
