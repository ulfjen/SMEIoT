using System.ComponentModel.DataAnnotations;

namespace SMEIoT.Web.BindingModels
{
  public class DeviceConfigBindingModel 
  {
    [Required]
    public string Name { get; set; }
    [Required]
    public string Identity { get; set; }
    [Required]
    public string Key { get; set; }
  }
}
