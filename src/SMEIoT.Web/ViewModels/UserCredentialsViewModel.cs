using System.ComponentModel.DataAnnotations;

namespace SMEIoT.Models.AccountViewModels
{
  public class UserCredentialsViewModel
  {
    [Required] public string Username { get; set; }

    [Required] public string Password { get; set; }
  }
}
