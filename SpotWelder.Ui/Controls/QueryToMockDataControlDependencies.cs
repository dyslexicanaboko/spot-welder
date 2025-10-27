using Microsoft.Extensions.Logging;
using SpotWelder.Lib.DataAccess;
using SpotWelder.Lib.DataAccess.SqlClients;
using SpotWelder.Lib.Services;
using SpotWelder.Lib.Services.TableQueryFormats;
using SpotWelder.Ui.Profile;

namespace SpotWelder.Ui.Controls
{
  public class QueryToMockDataControlDependencies
  {
    public QueryToMockDataControlDependencies(
      ILogger<QueryToMockDataControl> logger,
      ITableQueryFormatFactory tableQueryFormatFactory,
      IQueryToMockDataService queryToMockDataService,
      IGeneralDatabaseQueries repository,
      IProfileManager profileManager,
      IConnectionStringBuilderService builderService,
      ConnectionStringControlDependencies connectionStringControlDependencies)
    {
      Logger = logger;
      TableQueryFormatFactory = tableQueryFormatFactory;
      QueryToMockDataService = queryToMockDataService;
      Repository = repository;
      ProfileManager = profileManager;
      BuilderService = builderService;
      ConnectionStringControlDependencies = connectionStringControlDependencies;
    }

    public ILogger<QueryToMockDataControl> Logger { get; }

    public ITableQueryFormatFactory TableQueryFormatFactory { get; set; }
    public IQueryToMockDataService QueryToMockDataService { get; set; }
    public IGeneralDatabaseQueries Repository { get; set; }
    public IProfileManager ProfileManager { get; set; }
    public IConnectionStringBuilderService BuilderService { get; set; }

    public ConnectionStringControlDependencies ConnectionStringControlDependencies { get; set; }
  }
}
