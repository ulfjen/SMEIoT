using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using NodaTime;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;
using SMEIoT.Core.Exceptions;
using SMEIoT.Web.ApiModels;
using System.ComponentModel;
using System;

namespace SMEIoT.Web.Api.V1
{
  [Authorize]
  public class SettingsController : BaseController 
  {
    private readonly ILogger _logger;
    private readonly ISettingsService<Settings> _service;

    public SettingsController(
      ILogger<SettingsController> logger,
      ISettingsService<Settings> service)
    {
      _logger = logger;
      _service = service;
    }

    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<SettingApiModelList>> Index()
    {
      var list = new List<SettingApiModel>();
      
      var descriptors = await _service.ListSettingDescriptorsAsync();
      list.AddRange(descriptors.Select(d => new SettingApiModel(d)));

      return Ok(new SettingApiModelList(list));
    }
  }
}
