using System.ComponentModel.DataAnnotations;

namespace SMEIoT.Web.BindingModels
{
  public class LoginBindingModel
  {
    [Required] public string Username { get; set; }

    [Required] public string Password { get; set; }
    
    [Url]
    public string? ReturnUrl { get; set; }
  }
}
