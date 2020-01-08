using System.ComponentModel.DataAnnotations;

namespace SMEIoT.Web.BindingModels
{
  public class AssignUserSensorBindingModel
  {
    [Required]
    public string UserName { get; set; } = string.Empty;
  }
}
