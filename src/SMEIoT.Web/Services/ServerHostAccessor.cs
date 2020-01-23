using System;
using System.Threading.Tasks;
using SMEIoT.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.HostFiltering;

namespace SMEIoT.Web.Services
{
  public class ServerHostAccessor : IServerHostAccessor
  {
    private readonly IServerNetworkInterfacesIpAccessor _accessor;
    private readonly IOptions<HostFilteringOptions> _options;

    public ServerHostAccessor(IServerNetworkInterfacesIpAccessor accessor, IOptions<HostFilteringOptions> options)
    {
      _accessor = accessor;
      _options = options;
    }

    public Task<string?> GetServerHostAsync()
    {
      var domains = _options.Value.AllowedHosts;
      if (domains != null) {
        foreach (var d in domains) {
          if (d != "*") {
            return Task.FromResult<string?>(d);
          }
        }
      }
      
      var ips = _accessor.GetNetworkInterfacesIpString();
      if (ips.Count > 0) {
        return Task.FromResult<string?>(ips[0]);
      }

      return Task.FromResult<string?>(null);
    }
  }
}
