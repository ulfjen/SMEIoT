using SMEIoT.Core.Entities;
using NodaTime;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class BasicDeviceApiModel
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public string Name { get; }

    public Instant CreatedAt { get; }

    public Instant UpdatedAt { get; }

    public DeviceAuthenticationType AuthenticationType { get; }

    [JsonProperty(Required = Required.DisallowNull)]
    public string? PreSharedKey { get; }

    public bool Connected { get; }

    public Instant? ConnectedAt { get; }

    public Instant? LastMessageAt { get; }

    public BasicDeviceApiModel(Device device)
    {
      Name = device.Name;
      CreatedAt = device.CreatedAt;
      UpdatedAt = device.UpdatedAt;
      AuthenticationType = device.AuthenticationType;
      PreSharedKey = device.PreSharedKey;
      Connected = device.Connected;
      ConnectedAt = device.ConnectedAt;
      LastMessageAt = device.LastMessageAt;
    }
  }
}
