using System;
using System.Data;
using Dapper;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Infrastructure.Data
{
  class ApplicationDbConnection: IApplicationDbConnection
  {
    private readonly IDbConnection _dbConnection;

    public ApplicationDbConnection(IDbConnection dbConnection)
    {
      _dbConnection = dbConnection;
    }

    public string ConnectionString { get => _dbConnection.ConnectionString; set => _dbConnection.ConnectionString = value; }

    public int ConnectionTimeout => _dbConnection.ConnectionTimeout;

    public string Database => _dbConnection.Database;

    public ConnectionState State => _dbConnection.State;

    public IDbTransaction BeginTransaction()
    {
      return _dbConnection.BeginTransaction();
    }

    public IDbTransaction BeginTransaction(IsolationLevel il)
    {
      return _dbConnection.BeginTransaction(il);
    }

    public void ChangeDatabase(string databaseName)
    {
      _dbConnection.ChangeDatabase(databaseName);
    }

    public void Close()
    {
      _dbConnection.Close();
    }

    public IDbCommand CreateCommand()
    {
      return _dbConnection.CreateCommand();
    }

    public void Dispose()
    {
      _dbConnection.Dispose();
    }

    public void Open()
    {
      _dbConnection.Open();
    }

    public T ExecuteScalar<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
      return _dbConnection.ExecuteScalar<T>(sql, param, transaction, commandTimeout, commandType);
    }
  }
}
