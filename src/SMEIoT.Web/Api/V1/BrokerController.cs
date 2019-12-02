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
  public class BrokerController : BaseController
  {
    private readonly ILogger<BrokerController> _logger;
    private readonly IMosquittoBrokerService _service;

    public BrokerController(
      ILogger<BrokerController> logger,
      IMosquittoBrokerService service)
    {
      _logger = logger;
      _service = service;
    }

    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BrokerDetailsApiModel>> Index()
    {
      var statistics = _service.ListBrokerStatistics();
      var res = new BrokerDetailsApiModel(statistics, _service.BrokerRunning, _service.BrokerLastUpdatedAt);
      return Ok(res);
    }
  }
}
