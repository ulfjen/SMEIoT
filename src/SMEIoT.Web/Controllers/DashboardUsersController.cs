using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMEIoT.Core.Interfaces;
using SMEIoT.Web.ApiModels;

namespace SMEIoT.Web.Controllers
{
  public class DashboardUsersController : Controller
  {
    private IUserManagementService _service;

    public DashboardUsersController(IUserManagementService service)
    {
      _service = service;
    }
    
    [HttpGet]
    [Route("dashboard/users/{username}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditUser([Required] string username)
    {
      var (user, roles) = await _service.GetUserAndRoleByName(username);
      return View(new AdminUserApiModel(user, roles));
    }

  }
}
