using System.Text.Json.Serialization;

namespace SMEIoT.Web.ApiModels
{
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public enum SensorStatus
  {
    // we got a message from mqtt
    NotRegistered,
    
    // our sensor is registered in db but not connected anymore
    NotConnected, 
    
    // registered in db and runs fine 
    Connected
  }
}
