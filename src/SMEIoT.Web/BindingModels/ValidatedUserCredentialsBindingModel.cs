using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace SMEIoT.Web.BindingModels
{
  public class ValidatedUserCredentialsBindingModel
  {
    [BindRequired]
    [JsonProperty(Required = Required.DisallowNull)]
    [Display(Name = "UserName")]
    public string UserName { get; set; } = null!;

    [BindRequired]
    [JsonProperty(Required = Required.DisallowNull)]
    [StringLength(1024, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    // TODO: constraints on 10000 common password
    public string Password { get; set; } = null!;
  }
}
