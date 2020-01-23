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

    [Display(Description = "The broker is expected to run with SMEIoT server. If that's not the case, toggle the setting to show real host from our MQTT client. With the setting on, we will try to get the binding host or local IP.")]
    [DefaultValue(true)]
    public bool ShowingLocalHostAsBrokerHost { get; set; }

    [StringLength(10240, ErrorMessage = "Setting for {0} must be shorter than {1}.")]
    [Display(Description = "Only watch messages from the broker under this topic branch. It's used for device & sensor discovery.")]
    [DefaultValue("iot/")]
    public string MqttSensorTopicPrefix { get; set; } = null!;

    public override void ValidateItems()
    {
      ValidateDeviceNameLength();
      ValidateSensorNameLength();
      ValidateMqttSensorTopicPrefix();
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

    private void ValidateMqttSensorTopicPrefix()
    {
      if (MqttSensorTopicPrefix.Trim().Length != MqttSensorTopicPrefix.Length) {
        throw new InvalidArgumentException($"{nameof(MqttSensorTopicPrefix)} can't has white space.", nameof(Settings));
      }
      if (MqttSensorTopicPrefix.StartsWith("$SYS")) {
        throw new InvalidArgumentException($"{nameof(MqttSensorTopicPrefix)} can't be with broker messages.", nameof(Settings));
      }
    }
  }
}
