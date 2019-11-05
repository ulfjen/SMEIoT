using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;
using SMEIoT.Web.Hubs;

namespace SMEIoT.Web.Services
{
  /// <summary>
  /// a singleton running on background to send messages to all clients
  /// </summary>
  public class MqttMessageHubDeliveryService : IMqttMessageObserver
  {
    private readonly IHubContext<MqttHub> _hubContext;

    public MqttMessageHubDeliveryService(IHubContext<MqttHub> hubContext)
    {
      _hubContext = hubContext;
    }

    public void Update(MqttMessage message)
    {
      _hubContext.Clients.All.SendAsync("ReceiveMessage", new
      {
        Message = $"[{message.ReceivedAt.ToString()}] {message.Topic}: {message.Payload}"
      }).ConfigureAwait(false).GetAwaiter().GetResult();
    }
  }
}
