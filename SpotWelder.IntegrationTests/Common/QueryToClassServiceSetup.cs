using SpotWelder.Lib;
using SpotWelder.Lib.DataAccess;
using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services;
using SpotWelder.Lib.Services.CodeFactory;
using SpotWelder.Lib.Services.Generators;
using SpotWelder.Lib.Services.TableQueryFormats;
using System;

namespace SpotWelder.IntegrationTests.Common
{
  public class QueryToClassServiceSetup
  {
    //Language, elections, server connection - these will probably have to be passed in later
    public QueryToClassParameters GetParameters(SqlEngine sqlEngine)
    {
      var sp = BuildParameters(sqlEngine);

      var p = new QueryToClassParameters();

      p.LanguageType = CodeType.CSharp;
      p.OverwriteExistingFiles = true;
      p.Namespace = "NoOneCares";
      p.SubjectName = "DataTypeTest";
      p.EntityName = "DataTypeTestEntity";
      p.ModelName = "DataTypeTestModel";
      p.ServerConnection.SqlEngine = sqlEngine;
      p.ServerConnection.ConnectionString = sp.ConnectionString;
      p.ServerConnection.SourceSqlType = SourceSqlType.TableName;
      p.ServerConnection.SourceSqlText = sp.FullTableName;
      p.ServerConnection.TableQuery = sp.NameFormatService.ParseTableName(p.ServerConnection.SourceSqlText);

      return p;
    }

    private SqlEngineTestParameters BuildParameters(SqlEngine sqlEngine)
    {
      ITableQueryFormatStrategy svcNameFormat;
      string connectionString;
      string fullTableName;
      string sqlQuery;

      switch (sqlEngine)
      {
        case SqlEngine.SqlServer:
          svcNameFormat = new SqlServerTableQueryFormatStrategy();
          connectionString = "Server=.;Database=SpotWelder;Integrated Security=SSPI;Encrypt=False;";
          fullTableName = "dbo.DataTypeTest";
          sqlQuery = "SELECT * FROM dbo.DataTypeTest"; //Will have to update this later to include the columns
          break;
        case SqlEngine.Postgres:
          svcNameFormat = new PostgresTableQueryFormatStrategy();
          connectionString = "Host=localhost;Username=postgres;Password=postgres;Database=spotwelder";
          fullTableName = "public.data-type-test";
          sqlQuery = "SELECT * FROM public.data-type-test";
          break;
        default:
          throw new NotImplementedException($"SQL engine {sqlEngine} is not supported yet. 0x202506012228");
      }

      return new SqlEngineTestParameters
      {
        SqlEngine = sqlEngine,
        NameFormatService = svcNameFormat,
        ConnectionString = connectionString,
        FullTableName = fullTableName,
        SqlQuery = sqlQuery
      };
    }

    public IQueryToClassService GetQueryToClassService(QueryToClassParameters parameters)
    {
      //For each Election put the corresponding generator in the array


      return new QueryToClassService(
        new QueryToClassRepository(),
        new GeneralDatabaseQueries(),
        new CodeGenerationFactory(new GeneratorBase[] {
          //Array needs to be dynamic - whatever is chosen for the elections needs the corresponding generator
          new ClassEntityGenerator(),
          new MapperGenerator()
        }));
    }
  }

  public class SqlEngineTestParameters
  {
    public SqlEngine SqlEngine { get; set; }

    public ITableQueryFormatStrategy NameFormatService { get; set; }

    public string ConnectionString { get; set; }
    
    public string FullTableName { get; set; }
    
    public string SqlQuery { get; set; }
  }

}
