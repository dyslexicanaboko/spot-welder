using Microsoft.Data.SqlClient;
using Npgsql;
using SpotWelder.Lib.Models;
using System;

namespace SpotWelder.Lib.DataAccess.SqlClients
{
  public class ConnectionStringBuilderService
    : IConnectionStringBuilderService
  {
    public string Build(ConnectionStringMeta meta, SqlEngine sqlEngine)
    {
      if (sqlEngine == SqlEngine.SqlServer)
      {
        var sbS = new SqlConnectionStringBuilder();
        sbS.InitialCatalog = meta.DatabaseName;
        sbS.Encrypt = meta.IsEncrypted;
        sbS.IntegratedSecurity = meta.IsIntegratedSecurity;

        if (!meta.IsIntegratedSecurity)
        {
          //Only set the username and password if integrated security is false
          sbS.UserID = meta.Username;
          sbS.Password = meta.Password;
        }

        //Don't save the port as zero, just use the default port instead
        if (meta.Port == 0) meta.Port = Constants.PostgresDefaultPort;

        //Port number cannot be specified as its own property
        sbS.DataSource = meta.Port == Constants.SqlServerDefaultPort ? meta.ServerName : $"{meta.ServerName},{meta.Port}";

        return sbS.ToString();
      }

      var sbP = new NpgsqlConnectionStringBuilder();
      sbP.Host = meta.ServerName;
      sbP.Database = meta.DatabaseName;
      sbP.Username = meta.Username;
      sbP.Password = meta.Password;

      //Don't save the port as zero, just use the default port instead
      if (meta.Port == 0) meta.Port = Constants.PostgresDefaultPort;

      if (meta.Port != Constants.PostgresDefaultPort)
        sbP.Port = meta.Port;

      return sbP.ToString();
    }

    public ConnectionStringMeta Parse(string connectionString, SqlEngine sqlEngine)
    {
      if (sqlEngine == SqlEngine.SqlServer)
      {
        var sbS = new SqlConnectionStringBuilder(connectionString);

        var meta = new ConnectionStringMeta
        {
          DatabaseName = sbS.InitialCatalog,
          IsEncrypted = sbS.Encrypt,
          IsIntegratedSecurity = sbS.IntegratedSecurity,
          Username = sbS.UserID,
          Password = sbS.Password,
        };

        //If a port is specified, then this value has to be split
        if (sbS.DataSource.Contains(","))
        {
          var arr = sbS.DataSource.Split(",");
          meta.ServerName = arr[0];
          meta.Port = Convert.ToInt32(arr[1]);
        }
        else
        {
          meta.ServerName = sbS.DataSource;
          meta.Port = Constants.SqlServerDefaultPort;
        }

        return meta;
      }

      var sbP = new NpgsqlConnectionStringBuilder(connectionString);
      
      return new ConnectionStringMeta
      {
        ServerName = sbP.Host ?? string.Empty,
        DatabaseName = sbP.Database ?? string.Empty,
        Username = sbP.Username ?? string.Empty,
        Password = sbP.Password ?? string.Empty,
        Port = sbP.Port == 0 ? Constants.PostgresDefaultPort : sbP.Port
      };
    }
  }
}
