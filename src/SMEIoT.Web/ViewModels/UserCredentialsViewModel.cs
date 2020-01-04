using System.ComponentModel.DataAnnotations;

namespace SMEIoT.Web.ViewModels
{
  public class UserCredentialsViewModel
  {
    [Required] public string UserName { get; set; }

    [Required] public string Password { get; set; }
  }
}
