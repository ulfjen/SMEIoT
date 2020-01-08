using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SMEIoT.Core.Interfaces;
using SMEIoT.Web.ApiModels;

namespace SMEIoT.Web.Api.V1
{
  [ApiConventionType(typeof(DefaultApiConventions))]
  [ApiController]
  [Produces("application/json")]

  public class DevicesConfigController : ControllerBase
  {
    private readonly IDeviceSensorIdentifierSuggestService _identifierSuggestService;
    private readonly ISecureKeySuggestService _secureKeySuggestService;

    public DevicesConfigController(IDeviceSensorIdentifierSuggestService identifierSuggestService, ISecureKeySuggestService secureKeySuggestService)
    {
      _identifierSuggestService = identifierSuggestService;
      _secureKeySuggestService = secureKeySuggestService;
    }


    [HttpGet("api/devices/suggest_bootstrap_config")]
    public async Task<ActionResult<DeviceConfigSuggestApiModel>> SuggestBootstrap()
    {
      return Ok(new DeviceConfigSuggestApiModel() { DeviceName = _identifierSuggestService.GenerateRandomIdentifierForDevice(2), Key = _secureKeySuggestService.GenerateSecureKey(64) });
    }

    [HttpGet("api/devices/suggest_key")]
    public async Task<ActionResult<DeviceConfigSuggestApiModel>> SuggestSecureKey()
    {
      return Ok(new DeviceConfigSuggestApiModel() { Key = _secureKeySuggestService.GenerateSecureKey(64) });

    }
  }
}
