using System.ComponentModel.DataAnnotations;

namespace SMEIoT.Web.BindingModels
{
  public class DeviceBootstrapConfigBindingModel 
  {
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string Key { get; set; } = string.Empty;
  }
}
