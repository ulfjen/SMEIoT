using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Linq;
using NodaTime;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Mono.Posix;

namespace SMEIoT.Core.Services
{
  public class MosquittoBrokerService : IMosquittoBrokerService
  {
    private readonly ConcurrentDictionary<string, string> _values = new ConcurrentDictionary<string, string>();
    private readonly IClock _clock;
    private readonly ILogger _logger;

    public bool BrokerRunning {
      get {
        var brokerSendingMessage = BrokerLastMessageAt != null && _clock.GetCurrentInstant() - BrokerLastMessageAt <= Duration.FromSeconds(45);
        var brokerExist = BrokerPid != null && BrokerPid == BrokerPidFromAuthPlugin;
        // ensure we are talking with the same process
        return brokerSendingMessage && brokerExist;
      }
    }

    public Instant? BrokerLastMessageAt { get; set; }
    public int? BrokerPidFromAuthPlugin { get; set; }

    public int? BrokerPid => GetBrokerPidFromPidFile("/var/run/smeiot.mosquitto.pid");

    private const string ByteLoadReceived1Min = "load/bytes/received/1min";
    private const string ByteLoadReceived5Min = "load/bytes/received/5min";
    private const string ByteLoadReceived15Min = "load/bytes/received/15min";
    private const string ByteLoadSent1Min = "load/bytes/sent/1min";
    private const string ByteLoadSent5Min = "load/bytes/sent/5min";
    private const string ByteLoadSent15Min = "load/bytes/sent/15min";

    public MosquittoBrokerService(IClock clock, ILogger<MosquittoBrokerService> logger)
    {
      _clock = clock;
      _logger = logger;
    }

    public int? GetBrokerPidFromPidFile(string path)
    {
      try {
        var txt = File.ReadAllText(path);
        return int.Parse(txt);
      } catch {
        return null;
      }
    }

    private Task SendSignalAsync(int signal, bool ignoreAuthPluginPid)
    {
      if (BrokerPid == null || (!ignoreAuthPluginPid && BrokerPidFromAuthPlugin == null)) {
        return Task.FromException(new ArgumentException($"The broker is not running correctly. We get pid from the broker as {BrokerPid}. And the auth plugin says {BrokerPidFromAuthPlugin}."));
      }
      try {
        _logger.LogInformation($"We are sending a signal {signal} to {BrokerPid}");
        Syscall.kill(BrokerPid.Value, signal);
        return Task.CompletedTask;
      } catch (Exception ex) {
        return Task.FromException(ex);
      }
    }

    public Task ReloadBrokerBySignalAsync(bool ignoreAuthPluginPid = false) => SendSignalAsync((int) Signals.SIGHUP, ignoreAuthPluginPid);

    // do it by kill it and wait for systemd to bring the service online
    public Task RestartBrokerBySignalAsync(bool ignoreAuthPluginPid = false) => SendSignalAsync((int) Signals.SIGKILL, ignoreAuthPluginPid);

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
  }
}
