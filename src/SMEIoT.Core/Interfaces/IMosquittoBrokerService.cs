using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NodaTime;

namespace SMEIoT.Core.Interfaces
{
  public interface IMosquittoBrokerService
  {
    bool RegisterBrokerStatistics(string name, string value);
    string? GetBrokerStatistics(string name);
    IEnumerable<KeyValuePair<string, string>> ListBrokerStatistics();
    Tuple<double?, double?, double?> GetBrokerLoads();
    int? GetBrokerPidFromPidFile(string path);
    Task ReloadBrokerBySignalAsync(bool ignoreAuthPluginPid);
    Task RestartBrokerBySignalAsync(bool ignoreAuthPluginPid);

    bool BrokerRunning { get; }
    int? BrokerPid { get; }
    int? BrokerPidFromAuthPlugin { get; set; }
    Instant? BrokerLastUpdatedAt { get; set; }
  }
}
