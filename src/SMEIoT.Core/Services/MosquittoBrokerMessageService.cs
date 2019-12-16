using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SMEIoT.Core.Interfaces;
using SMEIoT.Core.Exceptions;

namespace SMEIoT.Core.Services
{
  class BrokerMeta
  {
    string MosquittoAuthPluginVersion { get; set; }
    int Pid { get; set; }
  }

  /// Broker will starts to ask us things. This is our message handler.
  public class MosquittoBrokerMessageService: IMosquittoBrokerMessageService
  {
    public const string META_CMD = "POSTMETA ";
    public const string PSK_CMD = "GETPSK ";
    public const string STATUS_OK = "OK";
    public const string STATUS_FAIL = "FAIL";

    private readonly IMosquittoClientAuthenticationService _clientService; // we need to subscribe the client as well as authenticate ourselves.
    private readonly IMosquittoBrokerService _brokerService;
    private readonly IDeviceService _deviceService;

    public MosquittoBrokerMessageService(IMosquittoClientAuthenticationService clientService, IMosquittoBrokerService brokerService, IDeviceService deviceService)
    {
      _clientService = clientService;
      _brokerService = brokerService;
      _deviceService = deviceService;
    }

    private Task<StringBuilder> HandleMetaCommandAsync(StringBuilder builder, string body)
    {
      var serializeOptions = new JsonSerializerOptions
      {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
      };
      var meta = JsonSerializer.Deserialize<BrokerMeta>(body, serializeOptions);
      _brokerService.BrokerPidFromAuthPlugin = meta.Pid;

      return Task.FromResult(builder.Append(STATUS_OK));
    }

    private async Task<StringBuilder> HandlePskCommandAsync(StringBuilder builder, string body)
    {
      if (body == _clientService.ClientName) {
        return builder.Append(_clientService.ClientPsk);
      }

      try {
        var device = await _deviceService.GetDeviceByNameAsync(body);
        if (device.PreSharedKey != null) {
          return builder.Append(device.PreSharedKey);
        }
      } catch (EntityNotFoundException _) {
        return await FailResponseAsync(builder);
      }

      return await FailResponseAsync(builder);
    }

    private Task<StringBuilder> FailResponseAsync(StringBuilder builder)
    {
      return Task.FromResult(builder.Append(STATUS_FAIL));
    }

    private Task<StringBuilder> HandleUnknownCommandAsync(StringBuilder builder) => FailResponseAsync(builder);

    public Task<StringBuilder> ProcessDecodedMessageAsync(string decoded)
    {
      var builder = new StringBuilder();
      if (decoded.StartsWith(META_CMD)) {
        return HandleMetaCommandAsync(builder, decoded.AsSpan().Slice(META_CMD.Length).ToString());
      }
      else if (decoded.StartsWith(PSK_CMD)) {
        return HandlePskCommandAsync(builder, decoded.AsSpan().Slice(PSK_CMD.Length).ToString());
      }
      return HandleUnknownCommandAsync(builder);
    }
  }
}
