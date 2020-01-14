using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using NodaTime;
using SMEIoT.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Mono.Unix.Native;

namespace SMEIoT.Core.Services
{
  public class MosquittoBrokerService : IMosquittoBrokerService
  {
    private readonly ConcurrentDictionary<string, string> _values = new ConcurrentDictionary<string, string>();
    private readonly IClock _clock;
    private readonly ILogger _logger;
    private readonly IMosquittoBrokerPidAccessor _accessor;
    private readonly IMosquittoBrokerPluginPidService _pluginService;

    public bool BrokerRunning {
      get {
        var brokerSendingMessage = BrokerLastMessageAt != null && _clock.GetCurrentInstant() - BrokerLastMessageAt <= Duration.FromSeconds(45);
        var brokerExist = _accessor.BrokerPid != null && _accessor.BrokerPid == _pluginService.BrokerPidFromAuthPlugin;
        _logger.LogTrace($"Broker last message {BrokerLastMessageAt}; Pid {_accessor.BrokerPid}; Plugin {_pluginService.BrokerPidFromAuthPlugin}");
        // ensure we are talking with the same process
        return brokerSendingMessage && brokerExist;
      }
    }

    public Instant? BrokerLastMessageAt { get; set; }

    private const string ByteLoadReceived1Min = "load/bytes/received/1min";
    private const string ByteLoadReceived5Min = "load/bytes/received/5min";
    private const string ByteLoadReceived15Min = "load/bytes/received/15min";
    private const string ByteLoadSent1Min = "load/bytes/sent/1min";
    private const string ByteLoadSent5Min = "load/bytes/sent/5min";
    private const string ByteLoadSent15Min = "load/bytes/sent/15min";

    public MosquittoBrokerService(IClock clock, ILogger<MosquittoBrokerService> logger, IMosquittoBrokerPidAccessor accessor, IMosquittoBrokerPluginPidService pluginService)
    {
      _clock = clock;
      _logger = logger;
      _accessor = accessor;
      _pluginService = pluginService;
    }


    private Task SendSignalAsync(Signum signal, bool ignoreAuthPluginPid)
    {
      if (_accessor.BrokerPid == null || (!ignoreAuthPluginPid && _pluginService.BrokerPidFromAuthPlugin == null)) {
        return Task.FromException(new ArgumentException($"The broker is not running correctly. We get pid from the broker as {_accessor.BrokerPid}. And the auth plugin says {_pluginService.BrokerPidFromAuthPlugin}."));
      }
      try {
        _logger.LogInformation($"We are sending a signal {signal} to {_accessor.BrokerPid}");
        Syscall.kill(_accessor.BrokerPid.Value, signal);
        return Task.CompletedTask;
      } catch (Exception ex) {
        return Task.FromException(ex);
      }
    }

    public Task ReloadBrokerBySignalAsync(bool ignoreAuthPluginPid = false) => SendSignalAsync(Signum.SIGHUP, ignoreAuthPluginPid);

    // do it by kill it and wait for systemd to bring the service online
    public Task RestartBrokerBySignalAsync(bool ignoreAuthPluginPid = false) => SendSignalAsync(Signum.SIGKILL, ignoreAuthPluginPid);

    public Task<bool> RegisterBrokerStatisticsAsync(string name, string value, Instant createdAt)
    {
      BrokerLastMessageAt = createdAt;
      if (_values.TryGetValue(name, out var stored))
      {
        return Task.FromResult(_values.TryUpdate(name, value, stored));
      }
      else
      {
        return Task.FromResult(_values.TryAdd(name, value)); 
      }
    }

    public Task<string?> GetBrokerStatisticsAsync(string name)
    {
      return Task.FromResult(GetStatisticsValue(name));
    }

    public Task<IEnumerable<KeyValuePair<string, string>>> ListBrokerStatisticsAsync()
    {
      return Task.FromResult(_values.ToArray().AsEnumerable());
    }

    private string? GetStatisticsValue(string name)
    {
      try {
        string? value;
        var got = _values.TryGetValue(name, out value);
        return got ? value : null;
      } catch (ArgumentNullException) {
        return null;
      }
    }

    public Task<Tuple<double?, double?, double?>> GetBrokerLoadAsync()
    {
      var min1 = TryGetAverageStatistics(new [] { ByteLoadReceived1Min, ByteLoadSent1Min });
      var min5 = TryGetAverageStatistics(new [] { ByteLoadReceived5Min, ByteLoadSent5Min });
      var min15 = TryGetAverageStatistics(new [] { ByteLoadReceived15Min, ByteLoadSent15Min });
 
      return Task.FromResult(Tuple.Create(min1, min5, min15));
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
