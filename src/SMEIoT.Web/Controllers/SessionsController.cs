using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using SMEIoT.Core.Entities;
using SMEIoT.Web.ApiModels;
using SMEIoT.Web.BindingModels;

namespace SMEIoT.Web.Controllers
{
  public class SessionsController : ControllerBase
  {
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger _logger;

    public SessionsController(
      SignInManager<User> signInManager,
      ILoggerFactory loggerFactory)
    {
      _logger = loggerFactory.CreateLogger<SessionsController>();
      _signInManager = signInManager;
    }
    
    private IActionResult RedirectToLocal(string returnUrl)
    {
      if (Url.IsLocalUrl(returnUrl))
      {
        return Redirect(returnUrl);
      }
      else
      {
        return RedirectToAction("/");
      }
    }

    [HttpDelete("/logout")]
    [HttpGet("/logout")]
    public IActionResult Destroy(string? returnUrl = null)
    {
      _signInManager.SignOutAsync();

      return returnUrl != null ? RedirectToLocal(returnUrl) : RedirectToLocal("/");
    }

  }
}
