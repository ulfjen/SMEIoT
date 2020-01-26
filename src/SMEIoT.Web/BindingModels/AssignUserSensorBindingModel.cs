using System.ComponentModel.DataAnnotations;

namespace SMEIoT.Web.BindingModels
{
  public class AssignUserSensorBindingModel
  {
    [Required]
    [Display(Name = "username")]
    public string UserName { get; set; } = string.Empty;
  }
}
