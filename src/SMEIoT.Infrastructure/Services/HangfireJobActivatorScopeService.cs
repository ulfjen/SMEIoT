using System;
using Microsoft.Extensions.DependencyInjection;

namespace SMEIoT.Infrastructure.Services
{
  public class HangfireJobActivatorScopeService : Hangfire.JobActivatorScope
  {
    private readonly IServiceScope _serviceScope;
    
    public HangfireJobActivatorScopeService(IServiceScope serviceScope)
    {
      if (serviceScope == null) throw new ArgumentNullException(nameof(serviceScope));
      _serviceScope = serviceScope;
    }

    public override object Resolve(Type type)
    {
      return _serviceScope.ServiceProvider.GetService(type);
    }
  }
}
