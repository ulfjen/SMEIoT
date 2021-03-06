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
    private readonly IMqttClientConfigService _configService;
    private readonly IServerHostAccessor _hostAccessor;
    private const bool ShowRealHostOrIp = true;

    public BrokerController(
      ILogger<BrokerController> logger,
      IMosquittoBrokerService service,
      IMqttClientConfigService configService,
      IServerHostAccessor hostAccessor)
    {
      _logger = logger;
      _service = service;
      _configService = configService;
      _hostAccessor = hostAccessor;
    }

    private async Task<MqttBrokerConnectionInformation> GetBrokerConnectionInfoAsync()
    {
      var info = await _configService.SuggestConfigAsync();
      var realHost = await _hostAccessor.GetServerHostAsync();
      if (ShowRealHostOrIp && realHost != null) {
        info.Host = realHost;
      }
      return info;
    }

    [HttpGet("basic")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BasicBrokerApiModel>> ShowBasic()
    {
      var load = await _service.GetBrokerLoadAsync();
      var info = await GetBrokerConnectionInfoAsync();
      var res = new BasicBrokerApiModel(_service.BrokerRunning, _service.BrokerLastMessageAt, load, info);
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
