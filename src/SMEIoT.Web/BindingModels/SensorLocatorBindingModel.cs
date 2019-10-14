using System.ComponentModel.DataAnnotations;

namespace SMEIoT.Web.BindingModels
{
  public class SensorLocatorBindingModel
  {
    [Required]
    public string Name { get; set; }
  }
}
