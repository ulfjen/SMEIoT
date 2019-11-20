using System.ComponentModel.DataAnnotations;

namespace SMEIoT.Web.BindingModels
{
  public class DeviceBootstrapConfigBindingModel
  {
    [Required]
    public string Identity { get; set; }
    [Required]
    public string Key { get; set; }
  }
}
