using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;
using SMEIoT.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace SMEIoT.Core.Services
{
  public class MqttMessageIngestionService : IMqttMessageIngestionService
  {
    private readonly IServiceScopeFactory _scopeFactory;

    public MqttMessageIngestionService(IServiceScopeFactory scopeFactory)
    {
      _scopeFactory = scopeFactory;
    }

    public async Task ProcessAsync(MqttMessage message)
    {
      throw new NotImplementedException();
    }
  }
}
