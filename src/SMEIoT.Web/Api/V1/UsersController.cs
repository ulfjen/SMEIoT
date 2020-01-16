using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [AllowAnonymous]
    public async Task<ActionResult<BasicUserApiModel>> Create([BindRequired] ValidatedUserCredentialsBindingModel user)
    {
      await _userService.CreateUserWithPasswordAsync(user.UserName, user.Password);
      var result = await GetBasicUserResultAsync(user.UserName);
      return CreatedAtAction(nameof(Show), new { userName = result.UserName }, result);
    }

    [HttpPut("{userName}/password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [Authorize]
    public async Task<ActionResult<UserCredentialsUpdateApiModel>> EditPassword(string userName,
      [BindRequired] ConfirmedUserCredentialsUpdateBindingModel binding)
    {
      await _userService.UpdateUserPasswordAsync(userName, binding.CurrentPassword, binding.NewPassword);
      var (user, roles) = await _userService.GetUserAndRoleByNameAsync(userName);
      var res = new UserCredentialsUpdateApiModel(user, roles) {PasswordUpdated = true};
      return Ok(res);
    }
    
    [HttpGet("{userName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<ActionResult<BasicUserApiModel>> Show(string userName)
    {
      var model = await GetBasicUserResultAsync(userName);
      return Ok(model);
    }

    private async Task<BasicUserApiModel> GetBasicUserResultAsync(string userName)
    {
      var (user, roles) = await _userService.GetUserAndRoleByNameAsync(userName);
      return new BasicUserApiModel(user, roles);
    }
  }
}
