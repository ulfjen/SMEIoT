using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SMEIoT.Web.Controllers
{

  public class SensorsController : Controller
  {
    private readonly ILogger<SensorsController> _logger;

    public SensorsController(ILogger<SensorsController> logger)
    {
      _logger = logger;
    }
    
    [HttpGet("/sensors/{name}")]
    public IActionResult Show(string name)
    {
      return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View();
    }
  }
}
