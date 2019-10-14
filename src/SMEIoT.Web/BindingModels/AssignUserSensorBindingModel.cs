using System.ComponentModel.DataAnnotations;

namespace SMEIoT.Web.BindingModels
{
  public class AssignUserSensorBindingModel
  {
    [Required] public string Username { get; set; }

  }
}
