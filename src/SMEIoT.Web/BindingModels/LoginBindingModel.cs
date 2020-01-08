using System.ComponentModel.DataAnnotations;

namespace SMEIoT.Web.BindingModels
{
  public class LoginBindingModel
  {
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
    
    [Url]
    public string? ReturnUrl { get; set; }
  }
}
