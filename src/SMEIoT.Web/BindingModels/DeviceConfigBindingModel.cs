﻿using System.ComponentModel.DataAnnotations;

namespace SMEIoT.Web.BindingModels
{
  public class DeviceConfigBindingModel 
  {
    [Required]
    public string Key { get; set; } = string.Empty;
  }
}
