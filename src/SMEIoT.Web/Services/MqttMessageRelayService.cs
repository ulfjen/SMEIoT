using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;
using SMEIoT.Web.Hubs;

namespace SMEIoT.Web.Services
{
  public class MqttMessageRelayService : IMqttMessageRelayService
  {
    private readonly IHubContext<MqttHub> _hubContext;

    public MqttMessageRelayService(IHubContext<MqttHub> hubContext)
    {
      _hubContext = hubContext;
    }

    public async Task RelayAsync(MqttMessage message)
    {
      await _hubContext.Clients.All.SendAsync("ReceiveMessage", new
      {
        Message = $"[{message.ReceivedAt.ToString()}] {message.Topic}: {message.Payload}"
      });
    }
  }
}
