using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SMEIoT.Web.Services;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Web.Hubs
{
  public class MqttHub : Hub
  {
    private readonly IMqttMessageRelayService _service;

    public MqttHub(IMqttMessageRelayService service)
    {
      _service = service;
    }
    /// <summary>
    /// template function. Not actually used. See MqttMessageHubDeliveryService for dispatching.
    /// </summary>
    /// <param name="messages"></param>
    /// <returns></returns>
    public async Task SendLogs(string log)
    {
      await Clients.All.SendAsync("ReceiveMessage", new
      {
        Message = log
      });
    }
  }
}
