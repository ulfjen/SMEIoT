using System.Data;

namespace SMEIoT.Core.Interfaces
{
  public interface IApplicationDbConnection : IDbConnection
  {
    T ExecuteScalar<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
  }
}
