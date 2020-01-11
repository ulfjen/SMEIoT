using System;

namespace SMEIoT.Core.Entities
{
  public class SettingItemDescriptor
  {
    public string Description { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string DefaultValue { get; set; } = null!;

    public string Value { get; set; } = null!;

    public string Name { get; set; } = null!;
  }
}
