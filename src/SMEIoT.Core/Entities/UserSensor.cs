namespace SMEIoT.Core.Entities
{
  public class UserSensor
  {
    public long UserId { get; set; }
    public User User { get; set; }
    
    public long SensorId { get; set; }
    public Sensor Sensor { get; set; }
  }
}
