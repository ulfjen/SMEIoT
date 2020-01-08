using System.ComponentModel.DataAnnotations;

namespace SMEIoT.Web.BindingModels
{
  public class SensorLocatorBindingModel
  {
    [Required]
    public string DeviceName { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;
  }
}
