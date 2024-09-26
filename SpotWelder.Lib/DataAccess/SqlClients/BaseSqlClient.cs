using System.Data;

namespace SpotWelder.Lib.DataAccess.SqlClients;

public abstract class BaseSqlClient
{
  public abstract IDbConnection GetDbConnection(string connectionString);

  public abstract IDbCommand GetDbCommand(string cmdText, IDbConnection connection);

  public abstract IDbDataAdapter GetDbDataAdapter(IDbCommand selectCommand);
}
