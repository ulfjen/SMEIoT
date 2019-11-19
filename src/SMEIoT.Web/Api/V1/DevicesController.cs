using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NodaTime;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;
using SMEIoT.Web.ApiModels;
using SMEIoT.Web.BindingModels;

namespace SMEIoT.Web.Api.V1
{
  public class DevicesController : BaseController 
  {
    private readonly ILogger<DevicesController> _logger;
    private readonly IDeviceService _service;

    public DevicesController(ILogger<SensorsController> logger, ISensorService service)
    {
      _logger = logger;
      _service = service;
    }

    [HttpPost("bootstrap")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DeviceApiModel>> BootstrapWithPreSharedKey(DeviceBootstrapConfigBindingModel view)
    {
      var deviceName = await _service.BootstrapDeviceWithPreSharedKeyAsync(view.Identity, view.Key);
      var deivce = await _service.GetDeviceByNameAsync(deviceName);
      var res = new DeviceApiModel(device);
      return CreatedAtAction(nameof(Create), res);
    }

    [HttpPut("{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DeviceApiModel>> Update(DeviceConfigBindingModel view)
    {
      var device = await _service.GetDeviceByNameAsync(view.Name);
      var res = new DeviceApiModel(device);
      return Ok(res);
    }
  }
}
