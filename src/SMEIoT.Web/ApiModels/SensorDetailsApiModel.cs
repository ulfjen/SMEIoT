using System;
using System.Linq;
using SMEIoT.Core.Entities;
using System.Collections.Generic;
using NodaTime;
using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class SensorDetailsApiModel
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public string DeviceName { get; set; }

    [JsonProperty(Required = Required.DisallowNull)]
    public string SensorName { get; set; }

    public SensorStatus Status { get; set; }

    [JsonProperty(Required = Required.DisallowNull)]
    public IEnumerable<NumberTimeSeriesApiModel> Data { get; set; }
    
    public Instant? StartedAt { get; set; }
    public Duration? Duration { get; set; }

    public SensorDetailsApiModel(Sensor sensor, IList<(double, Instant)>? values = null)
    {
      SensorName = sensor.Name;
      Status = sensor.Connected ? SensorStatus.Connected : SensorStatus.NotConnected;
      DeviceName = sensor.Device.Name;

      var intermediate = new List<NumberTimeSeriesApiModel>();
      if (values != null) {
        if (values.Count > 0) {
          StartedAt = values[0].Item2;
          Duration = values[values.Count-1].Item2 - StartedAt;
        }
        intermediate.AddRange(values.Select(v => new NumberTimeSeriesApiModel(v)));
      }
      Data = intermediate;
    }

    public SensorDetailsApiModel(string sensorName, string deviceName)
    {
      SensorName = sensorName;
      Status = SensorStatus.NotRegistered;
      Data = Array.Empty<NumberTimeSeriesApiModel>();
      DeviceName = deviceName;
    }
  }
}
