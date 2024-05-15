using System.Data;
using SpotWelder.Lib.Models;

namespace SpotWelder.Lib.DataAccess
{
    public interface IGeneralDatabaseQueries
        : IBaseRepository
    {
        ConnectionResult TestConnectionString(string connectionString);

        DataTable GetRowData(string sql);
    }
}