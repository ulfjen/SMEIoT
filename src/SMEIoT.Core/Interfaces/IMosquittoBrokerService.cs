using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NodaTime;

namespace SMEIoT.Core.Interfaces
{
  public interface IMosquittoBrokerService
  {
    Task<bool> RegisterBrokerStatisticsAsync(string name, string value, Instant createdAt);
    Task<string?> GetBrokerStatisticsAsync(string name);
    Task<IEnumerable<KeyValuePair<string, string>>> ListBrokerStatisticsAsync();
    Task<Tuple<double?, double?, double?>> GetBrokerLoadAsync();

    /// <summary>
    /// reload/restart depends on the platform
    /// </summary>
    /// <param name="ignoreAuthPluginPid"></param>
    /// <returns></returns>
    Task ReloadBrokerBySignalAsync(bool ignoreAuthPluginPid);
    Task RestartBrokerBySignalAsync(bool ignoreAuthPluginPid);

    Task<(string, int)> GetClientConnectionInfoAsync();

    bool BrokerRunning { get; }
    Instant? BrokerLastMessageAt { get; set; }
  }
}
