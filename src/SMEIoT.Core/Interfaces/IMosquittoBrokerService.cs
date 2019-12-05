using System;
using System.Collections.Generic;
using NodaTime;

namespace SMEIoT.Core.Interfaces
{
  public interface IMosquittoBrokerService
  {
    bool RegisterBrokerStatistics(string name, string value);
    string? GetBrokerStatistics(string name);
    IEnumerable<KeyValuePair<string, string>> ListBrokerStatistics();
    Tuple<double?, double?, double?> GetBrokerLoads();

    bool SetBrokerRunningStatus(bool running, Instant lastUpdatedAt);

    bool BrokerRunning { get; }
    Instant? BrokerLastUpdatedAt { get; }
  }
}
