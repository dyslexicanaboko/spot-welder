using SpotWelder.Lib.DataAccess.SqlClients;
using SpotWelder.Lib.Models;
using System;
using System.Data;

namespace SpotWelder.Lib.DataAccess
{
  /// <summary>
  ///   The base Data Access Layer
  ///   All Data Access Layers should inherit from this base class
  /// </summary>
  public abstract class BaseRepository
    : IBaseRepository
  {
    private BaseSqlClient? _sqlClient;
    private ServerConnection? _serverConnection;

    protected BaseSqlClient SqlClient
    {
      get 
      { 
        ValidateServerConnection();

        return _sqlClient;
      }
    }

    public void ConfigureSqlClient(ServerConnection serverConnection)
    {
      _serverConnection = serverConnection;
      
      _sqlClient = _serverConnection.SqlEngine switch
      {
        SqlEngine.SqlServer => new SqlServerClient(),
        SqlEngine.Postgres => new PostgresClient(),
        _ => throw new NotImplementedException("Selected SQL engine not supported yet. 0x202409252104")
      };
    }

    private void ValidateServerConnection()
    {
      if (_serverConnection == null || _sqlClient == null)
        throw new InvalidOperationException("You must configure the SQL client first. 0x202409252113");
    }

    protected SchemaRaw GetFullSchemaInformation(string sql)
    {
      ValidateServerConnection();

      using var con = _sqlClient!.GetDbConnection(_serverConnection!.ConnectionString);

      using var cmd = _sqlClient.GetDbCommand(sql, con);

      con.Open();
      
      using var dr = cmd.ExecuteReader();

      var schemaTable = dr.GetSchemaTable();

      //This should never happen
      if (schemaTable == null) throw new InvalidOperationException("Schema table cannot be null. 0x202409252126");

      var dt1 = new DataTable();
      dt1.Load(dr);
      
      var da = _sqlClient.GetDbDataAdapter(cmd);

      var ds = new DataSet();

      da.FillSchema(ds, SchemaType.Source);

      return new SchemaRaw(ds.Tables[0], schemaTable);
    }

    protected object? ExecuteScalar(string sql)
    {
      ValidateServerConnection();
      
      using var con = _sqlClient!.GetDbConnection(_serverConnection!.ConnectionString);

      con.Open();

      using var cmd = _sqlClient.GetDbCommand(sql, con);

      cmd.CommandTimeout = 0;

      return cmd.ExecuteScalar();
    }

    protected virtual DataTable ExecuteDataTable(string sql)
    {
      ValidateServerConnection();
      
      using var con = _sqlClient!.GetDbConnection(_serverConnection!.ConnectionString);

      con.Open();

      using var cmd = _sqlClient.GetDbCommand(sql, con);

      cmd.CommandTimeout = 0;

      var ds = new DataSet();

      var da = _sqlClient.GetDbDataAdapter(cmd);

      da.Fill(ds);

      return ds.Tables[0];
    }

    public ConnectionResult TestConnectionString()
    {
      var result = new ConnectionResult();

      try
      {
        var obj = ExecuteScalar("SELECT 1;");

        result.Success = Convert.ToInt32(obj) == 1;
      }
      catch (Exception ex)
      {
        result.Success = false;
        result.ReturnedException = ex;
      }

      return result;
    }
  }
}


/*
// Not sure why I kept this around, but I will keep it here just in case for now

//protected IDataReader ExecuteStoredProcedure(string storedProcedure, params SqlParameter[] parameters)
//{
//  var con = new SqlConnection(_connectionString);

//  con.Open();

//  var cmd = new SqlCommand(storedProcedure, con);
//  cmd.CommandType = CommandType.StoredProcedure;
//  cmd.CommandTimeout = 0;

//  if (parameters.Any()) cmd.Parameters.AddRange(parameters);

//  var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

//  return reader;
//}

 */