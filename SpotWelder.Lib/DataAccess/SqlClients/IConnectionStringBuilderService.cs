using SpotWelder.Lib.Models;

namespace SpotWelder.Lib.DataAccess.SqlClients;

public interface IConnectionStringBuilderService
{
  string Build(ConnectionStringMeta meta, SqlEngine sqlEngine);

  ConnectionStringMeta Parse(string connectionString, SqlEngine sqlEngine);
}
