using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SMEIoT.Web.Controllers
{
  [Authorize]
  public class DashboardController : Controller
  {
    [HttpGet]
    public IActionResult Index()
    {
      return View();
    }
    
    [HttpGet]
    public IActionResult Sensors()
    {
      return View();
    } 
    
    [HttpGet]
    public IActionResult Users()
    {
      return View();
    }
    
  }
}
