using System.Data.Common;
using Microsoft.Extensions.Configuration;

namespace SMEIoT.Infrastructure
{
  public static class DbConnectionStringExtensions
  {
    public static string BuildConnectionString(this IConfiguration configuration)
    {
      var builder = new DbConnectionStringBuilder
      {
        {"Host", configuration.GetConnectionString("Host")},
        {"Port", configuration.GetConnectionString("Port")},
        {"Database", configuration.GetConnectionString("Database")},
        {"Username", configuration.GetConnectionString("User")},
        {"Password", configuration.GetConnectionString("Password")}
      };
      return builder.ConnectionString;
    }
  }
}
