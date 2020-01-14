namespace SMEIoT.Core.Interfaces
{
  /// <summary>
  /// used to compare with broker pid we get from file system
  /// </summary>
  public interface IMosquittoBrokerPluginPidService
  {
    int? BrokerPidFromAuthPlugin { get; set; }

  }
}
