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
    Task<IEnumerable<KeyValuePair<string, string>>> ListBrokerStatisticsAsync();
    Task<Tuple<double?, double?, double?>> GetBrokerLoadAsync();
    int? GetBrokerPidFromPidFile(string path);
    Task ReloadBrokerBySignalAsync(bool ignoreAuthPluginPid);
    Task RestartBrokerBySignalAsync(bool ignoreAuthPluginPid);

    bool BrokerRunning { get; }
    int? BrokerPid { get; }
    int? BrokerPidFromAuthPlugin { get; set; }
    Instant? BrokerLastMessageAt { get; set; }
  }
}
