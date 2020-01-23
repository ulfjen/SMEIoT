using NodaTime;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SMEIoT.Core.Interfaces;
using SMEIoT.Core.Exceptions;
using System.Threading.Tasks;

namespace SMEIoT.Core.Entities
{
  public partial class Settings : SettingsBase
  {
    [Range(1, 5000, ErrorMessage = "Setting for {0} must be between {1} and {2}.")]
    [Display(Description = "Milliseconds between each request to the mosquitto broker.")]
    [DefaultValue(500)]
    public int MosquittoClientRunLoopInterval { get; set; }

    [Range(1, 200, ErrorMessage = "Setting for {0} must be between {1} and {2}.")]
    [Display(Description = "Minimum device name length. This setting must be between 1 and 200. And it must not be larger than MaximumDeviceNameLength.")]
    [DefaultValue(2)]
    public int MinimumDeviceNameLength { get; set; }

    [Range(1, 200, ErrorMessage = "Setting for {0} must be between {1} and {2}.")]
    [Display(Description = "Maximum device name length. This setting must be between 1 and 200.")]
    [DefaultValue(16)]
    public int MaximumDeviceNameLength { get; set; }

    [Range(1, 200, ErrorMessage = "Setting for {0} must be between {1} and {2}.")]
    [Display(Description = "Minimum sensor name length. This setting must be between 1 and 200. And it must not be larger than MaximumSensorNameLength.")]
    [DefaultValue(2)]
    public int MinimumSensorNameLength { get; set; }

    [Range(1, 200, ErrorMessage = "Setting for {0} must be between {1} and {2}.")]
    [Display(Description = "Maximum sensor name length. This setting must be between 1 and 200.")]
    [DefaultValue(16)]
    public int MaximumSensorNameLength { get; set; }

    public override void ValidateItems()
    {
      ValidateDeviceNameLength();
      ValidateSensorNameLength();
    }

    private void ValidateDeviceNameLength()
    {
      if (MaximumDeviceNameLength < MinimumDeviceNameLength) {
        throw new InvalidArgumentException($"{nameof(MinimumDeviceNameLength)} must not be larger than {nameof(MaximumDeviceNameLength)}", nameof(Settings));
      }
    }

    private void ValidateSensorNameLength()
    {
      if (MaximumSensorNameLength < MinimumSensorNameLength) {
        throw new InvalidArgumentException($"{nameof(MinimumSensorNameLength)} must not be larger than {nameof(MaximumSensorNameLength)}", nameof(Settings));
      }
    }
  }
}
