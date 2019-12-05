using System.Collections.Concurrent;
using System.Collections.Generic;
using System;
using System.Linq;
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
    private const string ByteLoadReceived1Min = "load/bytes/received/1min";
    private const string ByteLoadReceived5Min = "load/bytes/received/5min";
    private const string ByteLoadReceived15Min = "load/bytes/received/15min";
    private const string ByteLoadSent1Min = "load/bytes/sent/1min";
    private const string ByteLoadSent5Min = "load/bytes/sent/5min";
    private const string ByteLoadSent15Min = "load/bytes/sent/15min";

    public bool RegisterBrokerStatistics(string name, string value)
    {
      return _values.TryAdd(name, value);
    }

    public string? GetBrokerStatistics(string name)
    {
      return GetStatisticsValue(name);
    }

    public IEnumerable<KeyValuePair<string, string>> ListBrokerStatistics()
    {
      return _values.ToArray();
    }

    private string? GetStatisticsValue(string name)
    {
      string value;
      var got = _values.TryGetValue(name, out value);
      return got ? value : null;
    }

    public Tuple<double?, double?, double?> GetBrokerLoads()
    {
      var min1 = TryGetAverageStatistics(new [] { ByteLoadReceived1Min, ByteLoadSent1Min });
      var min5 = TryGetAverageStatistics(new [] { ByteLoadReceived5Min, ByteLoadSent5Min });
      var min15 = TryGetAverageStatistics(new [] { ByteLoadReceived15Min, ByteLoadSent15Min });
 
      return Tuple.Create(min1, min5, min15);
    }

    private double? TryGetAverageStatistics(string[] names)
    {
      var values = names.Select(n => GetStatisticsValueDouble(n)).ToArray();
      if (values.Any(v => v == null)) { return null; }
      return values.Sum() / values.Length;
    }

    private double? GetStatisticsValueDouble(string name)
    {
      var value = GetStatisticsValue(name);
      return value == null ? null : ConvertDoubleStatistics(value);
    }

    private double? ConvertDoubleStatistics(string value)
    {
      double number;
      var got = Double.TryParse(value, out number);
      return got ? (double?)number : null;
    }

    public bool SetBrokerRunningStatus(bool running, Instant lastUpdatedAt)
    {
      BrokerRunning = running;
      BrokerLastUpdatedAt = lastUpdatedAt;
      return true;
    }
  }
}
