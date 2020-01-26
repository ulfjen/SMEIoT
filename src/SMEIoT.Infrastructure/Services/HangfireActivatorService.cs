using System;
using Microsoft.Extensions.DependencyInjection;
using Hangfire;

namespace SMEIoT.Infrastructure.Services
{
  public class HangfireActivatorService : Hangfire.JobActivator
  {
    private readonly IServiceScopeFactory _serviceScopeFactory;
    public HangfireActivatorService(IServiceScopeFactory serviceScopeFactory)
    {
      if (serviceScopeFactory == null) throw new ArgumentNullException(nameof(serviceScopeFactory));
      _serviceScopeFactory = serviceScopeFactory;
    }

    public override JobActivatorScope BeginScope(JobActivatorContext context)
    {
      return new HangfireJobActivatorScopeService(_serviceScopeFactory.CreateScope());
    }
  }
}
