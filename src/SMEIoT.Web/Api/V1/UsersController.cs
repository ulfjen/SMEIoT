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
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<ActionResult<BasicUserApiModel>> Create(ValidatedUserCredentialsBindingModel user)
    {
      await _userService.CreateUserWithPassword(user.UserName, user.Password);
      var result = await GetBasicUserResultAsync(user.UserName);
      return CreatedAtAction(nameof(Show), result);
    }

    [HttpPut("{userName}/password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserCredentialsUpdateApiModel>> EditPassword(
      ConfirmedUserCredentialsUpdateBindingModel binding, string userName)
    {
      await _userService.UpdateUserPassword(userName, binding.CurrentPassword, binding.NewPassword);
      var (user, roles) = await _userService.GetUserAndRoleByName(userName);
      var res = new UserCredentialsUpdateApiModel(user, roles) {PasswordUpdated = true};
      return Ok(res);
    }
    
    [HttpGet("{userName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BasicUserApiModel>> Show(string userName)
    {
      var model = await GetBasicUserResultAsync(userName);
      return Ok(model);
    }

    private async Task<BasicUserApiModel> GetBasicUserResultAsync(string userName)
    {
      var (user, roles) = await _userService.GetUserAndRoleByName(userName);
      return new BasicUserApiModel(user, roles);
    }
  }
}
