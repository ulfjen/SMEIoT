using System.ComponentModel.DataAnnotations;

namespace SMEIoT.Web.BindingModels
{
  public class DeviceBootstrapConfigBindingModel 
  {
    [Required]
    [StringLength(1000, ErrorMessage = "Device name must be at least {2} characters long and no longer than {1} charcters.", MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(128, ErrorMessage = "Device key must be at least {2} characters long and no longer than {1} charcters.", MinimumLength = 64)]
    public string Key { get; set; } = string.Empty;
  }
}
