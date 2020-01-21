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
  }
}
