﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace SMEIoT.Web.BindingModels
{
  public class ValidatedUserCredentialsBindingModel
  {
    [BindRequired]
    [JsonProperty(Required = Required.DisallowNull)]
    [StringLength(1000, ErrorMessage = "The username must be at least {2} characters long.", MinimumLength = 3)]
    [Display(Name = "UserName")]
    public string UserName { get; set; } = null!;

    [BindRequired]
    [JsonProperty(Required = Required.DisallowNull)]
    [StringLength(1000, ErrorMessage = "The password must be at least {2} characters long.", MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    // TODO: constraints on 10000 common password
    public string Password { get; set; } = null!;
  }
}
