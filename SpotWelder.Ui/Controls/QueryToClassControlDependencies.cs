using Microsoft.Extensions.Logging;
using SpotWelder.Lib.DataAccess;
using SpotWelder.Lib.DataAccess.SqlClients;
using SpotWelder.Lib.Services;
using SpotWelder.Lib.Services.TableQueryFormats;
using SpotWelder.Ui.Profile;

namespace SpotWelder.Ui.Controls
{
  public class QueryToClassControlDependencies
  {
    public QueryToClassControlDependencies(
      ILogger<QueryToClassControl> logger,
      ITableQueryFormatFactory tableQueryFormatFactory,
      IQueryToClassService queryToClassService,
      IGeneralDatabaseQueries repository,
      IProfileManager profileManager,
      IConnectionStringBuilderService builderService,
      ConnectionStringControlDependencies connectionStringControlDependencies)
    {
      Logger = logger;
      TableQueryFormatFactory = tableQueryFormatFactory;
      QueryToClassService = queryToClassService;
      Repository = repository;
      ProfileManager = profileManager;
      BuilderService = builderService;
      ConnectionStringControlDependencies = connectionStringControlDependencies;
    }

    public ILogger<QueryToClassControl> Logger { get; }

    public ITableQueryFormatFactory TableQueryFormatFactory { get; set; }
    public IQueryToClassService QueryToClassService { get; set; }
    public IGeneralDatabaseQueries Repository { get; set; }
    public IProfileManager ProfileManager { get; set; }
    public IConnectionStringBuilderService BuilderService { get; set; }

    public ConnectionStringControlDependencies ConnectionStringControlDependencies { get; set; }
  }
}
