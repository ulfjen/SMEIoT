using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SMEIoT.Web.BindingModels
{
  public class ConfirmedUserCredentialsUpdateBindingModel
  {
    [Required]
    [JsonProperty(Required = Required.DisallowNull)]
    [StringLength(1024, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "CurrentPassword")]
    public string CurrentPassword { get; set; }
    
    [Required]
    [JsonProperty(Required = Required.DisallowNull)]
    [StringLength(1024, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "NewPassword")]
    public string NewPassword { get; set; }
  }
}
