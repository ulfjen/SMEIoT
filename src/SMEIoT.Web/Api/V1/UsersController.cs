using System.Net.Mime;
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
  public class UsersController : BaseController
  {
    private readonly ILogger _logger;
    private readonly IUserManagementService _userService;

    public UsersController(
      ILogger<UsersController> logger, IUserManagementService userService)
    {
      _logger = logger;
      _userService = userService;
    }

    [HttpPost("")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<ActionResult<BasicUserApiModel>> Create(ValidatedUserCredentialsBindingModel user)
    {
      _logger.LogDebug($"Creates user {user.Username}");
      await _userService.CreateUserWithPassword(user.Username, user.Password);
      var result = await GetBasicUserResultAsync(user.Username);
      return CreatedAtAction(nameof(Show), result);
    }

    [HttpPut("{username}/password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserCredentialsUpdateApiModel>> EditPassword(
      ConfirmedUserCredentialsUpdateBindingModel binding, string username)
    {
      await _userService.UpdateUserPassword(username, binding.CurrentPassword, binding.NewPassword);
      var (user, roles) = await _userService.GetUserAndRoleByName(username);
      var res = new UserCredentialsUpdateApiModel(user, roles) {PasswordUpdated = true};
      return Ok(res);
    }
    
    [HttpGet("{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BasicUserApiModel>> Show(string username)
    {
      var model = await GetBasicUserResultAsync(username);
      return Ok(model);
    }

    private async Task<BasicUserApiModel> GetBasicUserResultAsync(string username)
    {
      var (user, roles) = await _userService.GetUserAndRoleByName(username);
      return new BasicUserApiModel(user, roles);
    }
  }
}
