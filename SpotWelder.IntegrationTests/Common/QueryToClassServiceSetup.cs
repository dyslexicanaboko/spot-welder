using SpotWelder.Lib;
using SpotWelder.Lib.DataAccess;
using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services;
using SpotWelder.Lib.Services.CodeFactory;
using SpotWelder.Lib.Services.Generators;
using SpotWelder.Lib.Services.TableQueryFormats;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace SpotWelder.IntegrationTests.Common
{
  public class QueryToClassServiceSetup
  {
    //Language, elections, server connection - these will probably have to be passed in later
    public QueryToClassParameters GetParameters(SqlEngine sqlEngine, bool makeAsync, GenerationElections election)
    {
      var sp = BuildParameters(sqlEngine);

      var p = new QueryToClassParameters();

      p.LanguageType = CodeType.CSharp;
      p.Namespace = "Namespace1";
      p.SubjectName = "DataTypeTest";
      p.ServerConnection.SqlEngine = sqlEngine;
      p.ServerConnection.ConnectionString = sp.ConnectionString;
      p.ServerConnection.SourceSqlType = SourceSqlType.TableName;
      p.ServerConnection.SourceSqlText = sp.FullTableName;
      p.ServerConnection.TableQuery = sp.NameFormatService.ParseTableName(p.ServerConnection.SourceSqlText);

      if(makeAsync)
      {
        p.Elections |= GenerationElections.MakeAsynchronous;
      }

      //Since I am testing every election, they all have to be set here since there are interdependencies.
      //The sub-elections are set here explicitly as they are dynamically selected based on the main election.
      p.Elections |= 
        GenerationElections.Interface | 
        GenerationElections.Entity | 
        GenerationElections.Model |
        GenerationElections.CreateModel | 
        GenerationElections.PatchModel |
        GenerationElections.EntityIEquatable |
        GenerationElections.EntityIComparable |
        GenerationElections.MapEntityToModel |
        GenerationElections.MapModelToEntity |
        GenerationElections.MapInterfaceToModel |
        GenerationElections.MapInterfaceToEntity |
        GenerationElections.MapCreateModelToEntity |
        GenerationElections.MapPatchModelToEntity |
        election;

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
          connectionString = "Host=localhost;Username=postgres;Password=postgres;Database=spot_welder";
          fullTableName = "public.data_type_test";
          sqlQuery = "SELECT * FROM public.data_type_test";
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

    public IQueryToClassService GetQueryToClassService(IServiceProvider provider, QueryToClassParameters parameters)
    {
      //For each Election put the corresponding generator in the array
      //Whatever is chosen for the elections needs the corresponding generator
      return new QueryToClassService(
        new QueryToClassRepository(),
        new GeneralDatabaseQueries(),
        new CodeGenerationFactory(provider.GetService<IEnumerable<GeneratorBase>>()));
    }

    public string GetExpectedResult(SqlEngine sqlEngine, bool isAsync, GenerationElections election)
    {
      //Example output directory:
      //C:\Dev\GitHub\spot-welder\SpotWelder.IntegrationTests\bin\Debug\net8.0\ModelGenerationTests\ExpectedModels\{EnumerationName}.cs.sqlServer.expected
      var strAsync = isAsync ? "async" : "sync";
      var strSqlEngine = (election.HasFlag(GenerationElections.RepoDapper) || election.HasFlag(GenerationElections.RepoStatic)) ?
        $".{sqlEngine.ToString().ToLower()}" : string.Empty;

      var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ModelGenerationTests", "ExpectedModels",
        $"{election}.{strAsync}{strSqlEngine}.expected");

      if (!File.Exists(path)) throw new FileNotFoundException($"Make sure expected result files are in the output path.", path);

      return File.ReadAllText(path);
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
}
