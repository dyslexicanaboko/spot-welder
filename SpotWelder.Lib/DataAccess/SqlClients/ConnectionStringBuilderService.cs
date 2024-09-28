using Microsoft.Data.SqlClient;
using Npgsql;
using SpotWelder.Lib.Models;

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
        sbS.DataSource = meta.ServerName;
        sbS.InitialCatalog = meta.DatabaseName;
        sbS.Encrypt = meta.IsEncrypted;
        sbS.IntegratedSecurity = meta.IsIntegratedSecurity;

        if (meta.IsIntegratedSecurity)
          return sbS.ToString();

        //Only set the username and password if integrated security is false
        sbS.UserID = meta.Username;
        sbS.Password = meta.Password;

        return sbS.ToString();
      }

      var sbP = new NpgsqlConnectionStringBuilder();
      sbP.Host = meta.ServerName;
      sbP.Database = meta.DatabaseName;
      sbP.Username = meta.Username;
      sbP.Password = meta.Password;

      return sbP.ToString();
    }

    public ConnectionStringMeta Parse(string connectionString, SqlEngine sqlEngine)
    {
      if (sqlEngine == SqlEngine.SqlServer)
      {
        var sbS = new SqlConnectionStringBuilder(connectionString);
      
        return new ConnectionStringMeta
        {
          ServerName = sbS.DataSource,
          DatabaseName = sbS.InitialCatalog,
          IsEncrypted = sbS.Encrypt,
          IsIntegratedSecurity = sbS.IntegratedSecurity,
          Username = sbS.UserID,
          Password = sbS.Password
        };
      }

      var sbP = new NpgsqlConnectionStringBuilder(connectionString);
      
      return new ConnectionStringMeta
      {
        ServerName = sbP.Host ?? string.Empty,
        DatabaseName = sbP.Database ?? string.Empty,
        Username = sbP.Username ?? string.Empty,
        Password = sbP.Password ?? string.Empty
      };
    }
  }
}
