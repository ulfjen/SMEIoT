using System.ComponentModel.DataAnnotations;

namespace SMEIoT.Web.BindingModels
{
  public class LoginBindingModel
  {
    [Required]
    [Display(Name = "username")]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "password")]
    public string Password { get; set; } = string.Empty;
    
    [Url]
    public string? ReturnUrl { get; set; }
  }
}
