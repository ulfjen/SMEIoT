namespace SMEIoT.Web.ApiModels
{
  public class DeviceConfigSuggestApiModel
  {
    public string? DeviceName { get; set; }
    public string? Key { get; set; }
    public string? ContinuedConfigurationDevice { get; set; }

    public DeviceConfigSuggestApiModel(string? deviceName = null, string? key = null, string? continuedConfigurationDevice = null)
    {
      DeviceName = deviceName;
      Key = key;
      ContinuedConfigurationDevice = continuedConfigurationDevice;
    }
  }
}
