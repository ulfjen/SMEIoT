using NodaTime;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Core.Entities
{
  public class SettingItem : IAuditTimestamp
  {
    [Key]
    public string Name { get; set; } = null!;

    [Required]
    public byte[] Data { get; set; } = null!;

    [Required]
    public string Type { get; set; } = null!;

    public Instant CreatedAt { get; set; }
    public Instant UpdatedAt { get; set; }

    public static byte[] SerializeObjectToByteArray(object obj)
    {
      var formatter = new BinaryFormatter();
      var stream = new MemoryStream(16);
      formatter.Serialize(stream, obj);
      return stream.ToArray();
    }

    public object DeserializeDataToObject()
    {
      var formatter = new BinaryFormatter();
      var stream = new MemoryStream(Data);
      return formatter.Deserialize(stream);
    }
  }
}
