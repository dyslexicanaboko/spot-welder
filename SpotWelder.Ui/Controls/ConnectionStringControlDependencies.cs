using SpotWelder.Lib.DataAccess;
using SpotWelder.Lib.DataAccess.SqlClients;
using SpotWelder.Ui.Profile;

namespace SpotWelder.Ui.Controls
{
  public class ConnectionStringControlDependencies
  {
    public ConnectionStringControlDependencies(
      IProfileManager profileManager,
      IGeneralDatabaseQueries repository,
      IConnectionStringBuilderService builderService)
    {
      ProfileManager = profileManager;
      Repository = repository;
      BuilderService = builderService;
    }

    public IProfileManager ProfileManager { get; }

    public IGeneralDatabaseQueries Repository { get; }

    public IConnectionStringBuilderService BuilderService { get; set; }
  }
}
