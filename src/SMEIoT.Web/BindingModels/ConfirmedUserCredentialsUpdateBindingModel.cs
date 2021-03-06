﻿using System.ComponentModel.DataAnnotations;

namespace SMEIoT.Web.BindingModels
{
  public class ConfirmedUserCredentialsUpdateBindingModel
  {
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "current password")]
    public string CurrentPassword { get; set; } = string.Empty;
    
    [Required]
    [StringLength(1024, ErrorMessage = "Your new password must be at least {2} characters long.", MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "new password")]
    public string NewPassword { get; set; } = string.Empty;
  }
}
