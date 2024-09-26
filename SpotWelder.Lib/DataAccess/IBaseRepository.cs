using SpotWelder.Lib.Models;

namespace SpotWelder.Lib.DataAccess
{
  public interface IBaseRepository
  {
    void ConfigureSqlClient(ServerConnection serverConnection);
  }
}
