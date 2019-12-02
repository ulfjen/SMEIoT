using System.Collections.Concurrent;
using System.Collections.Generic;
using NodaTime;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Core.Services
{
  public class MosquittoBrokerService : IMosquittoBrokerService
  {
    private readonly ConcurrentDictionary<string, string> _values = new ConcurrentDictionary<string, string>();
    public bool BrokerRunning { get; private set; } = false;
    public Instant? BrokerLastUpdatedAt { get; private set; }

    public bool RegisterBrokerStatistics(string name, string value)
    {
      return _values.TryAdd(name, value);
    }

    public string? GetBrokerStatistics(string name)
    {
      string value;
      var got = _values.TryGetValue(name, out value);
      return got ? value : null;
    }

    public IEnumerable<KeyValuePair<string, string>> ListBrokerStatistics()
    {
      return _values.ToArray();
    }

    public bool SetBrokerRunningStatus(bool running, Instant lastUpdatedAt)
    {
      BrokerRunning = running;
      BrokerLastUpdatedAt = lastUpdatedAt;
      return true;
    }
  }
}
