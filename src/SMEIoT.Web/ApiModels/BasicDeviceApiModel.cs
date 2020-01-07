using SMEIoT.Core.Entities;
using NodaTime;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SMEIoT.Web.ApiModels
{
  public class BasicDeviceApiModel
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public string Name { get; }

    [JsonProperty(Required = Required.DisallowNull)]
    public Instant CreatedAt { get; }

    [JsonProperty(Required = Required.DisallowNull)]
    public Instant UpdatedAt { get; }

    [JsonProperty(Required = Required.DisallowNull)]
    public DeviceAuthenticationType AuthenticationType { get; }

    [JsonProperty(Required = Required.DisallowNull)]
    public string? PreSharedKey { get; }

    [JsonProperty(Required = Required.DisallowNull)]
    public bool Connected { get; }

    [JsonProperty(Required = Required.DisallowNull)]
    public Instant? ConnectedAt { get; }

    [JsonProperty(Required = Required.DisallowNull)]
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
