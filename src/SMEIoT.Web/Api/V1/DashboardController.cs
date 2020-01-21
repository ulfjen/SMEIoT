using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NodaTime;
using System;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;
using SMEIoT.Web.ApiModels;
using SMEIoT.Web.BindingModels;

namespace SMEIoT.Web.Api.V1
{
  public class DashboardController : BaseController
  {
    private readonly ILogger _logger;
    private readonly IDashboardService _service;

    public DashboardController(
      ILogger<DashboardController> logger,
      IDashboardService service)
    {
      _logger = logger;
      _service = service;
    }

    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<SystemHighlightsApiModel>> Show()
    {
      var highlights = await _service.GetSystemHighlightsAsync();
      var res = new SystemHighlightsApiModel(highlights);
      return Ok(res);
    }
  }
}
