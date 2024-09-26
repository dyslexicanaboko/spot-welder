using SpotWelder.Lib.Models;
using System.Data;

namespace SpotWelder.Lib.DataAccess
{
  public class GeneralDatabaseQueries
    : BaseRepository, IGeneralDatabaseQueries
  {
    public ConnectionResult TestConnectionString(ServerConnection serverConnection)
    {
      ConfigureSqlClient(serverConnection);

      return TestConnectionString();
    }

    public DataTable GetRowData(string sql) => ExecuteDataTable(sql);
  }
}
