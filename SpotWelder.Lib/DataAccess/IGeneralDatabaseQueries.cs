using SpotWelder.Lib.Models;
using System.Data;

namespace SpotWelder.Lib.DataAccess
{
  public interface IGeneralDatabaseQueries
    : IBaseRepository
  {
    ConnectionResult TestConnectionString(ServerConnection serverConnection);

    DataTable GetRowData(string sql);
  }
}
