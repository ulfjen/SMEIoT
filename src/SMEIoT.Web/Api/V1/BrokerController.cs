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
    private readonly ILogger _logger;
    private readonly IMosquittoBrokerService _service;

    public BrokerController(
      ILogger<BrokerController> logger,
      IMosquittoBrokerService service)
    {
      _logger = logger;
      _service = service;
    }

    [HttpGet("basic")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BasicBrokerApiModel>> ShowBasic()
    {
      var load = await _service.GetBrokerLoadAsync();
      var res = new BasicBrokerApiModel(_service.BrokerRunning, _service.BrokerLastMessageAt, load);
      return Ok(res);
    }

    [HttpGet("statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BrokerStatisticsApiModel>> ShowStatistics()
    {
      var statistics = await _service.ListBrokerStatisticsAsync();
      var res = new BrokerStatisticsApiModel(statistics);
      return Ok(res);
    }
  }
}
