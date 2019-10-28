using Microsoft.AspNetCore.Mvc;

namespace SMEIoT.Web.Api.V1
{
  [ApiConventionType(typeof(DefaultApiConventions))]
  [ApiController]
  [Produces("application/json")]
  [Route("api/[controller]")]
  public class BaseController : ControllerBase
  {
  }
}
