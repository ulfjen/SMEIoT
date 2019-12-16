namespace SMEIoT.Core.Interfaces
{
  public interface IMosquittoClientAuthenticationService
  {
    string ClientName { get; }
    string ClientPsk { get; }
  }
}
