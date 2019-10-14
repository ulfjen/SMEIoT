using System.ComponentModel.DataAnnotations;

namespace SMEIoT.Web.BindingModels
{
  public class ValidatedUserCredentialsBindingModel
  {
    [Required]
    [Display(Name = "Username")]
    public string Username { get; set; }

    [Required]
    [StringLength(1024, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    // TODO: constraints on 10000 common password
    public string Password { get; set; }
  }
}
