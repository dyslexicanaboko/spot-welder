using SpotWelder.Lib.DataAccess;
using SpotWelder.Lib.Models;
using System;
using System.Data;

namespace SpotWelder.Lib.Services
{
  public abstract class ClassMetaDataBase
  {
    private const string Select = "SELECT";

    protected readonly IGeneralDatabaseQueries _genericDatabaseQueries;

    protected readonly IQueryToClassRepository _queryToClassRepository;

    protected ClassMetaDataBase(IQueryToClassRepository repository, IGeneralDatabaseQueries genericDatabaseQueries)
    {
      _queryToClassRepository = repository;

      _genericDatabaseQueries = genericDatabaseQueries;
    }

    protected virtual SchemaQuery GetSchema(ServerConnection connection)
    {
      _queryToClassRepository.ConfigureSqlClient(connection);

      var schema = _queryToClassRepository.GetSchema(
        connection.TableQuery, 
        connection.SourceSqlType, 
        connection.SourceSqlText);

      return schema;
    }

    protected virtual DataTable GetRowData(ServerConnection connection, int? top = null)
    {
      var selector = string.Empty;
      var strTop = string.Empty;
      var sourceSqlText = connection.SourceSqlText;

      if (top.HasValue) strTop = $" TOP({top.Value}) ";

      if (connection.SourceSqlType == SourceSqlType.TableName)
      {
        selector = $"SELECT{strTop} * FROM ";
      }
      else if (top.HasValue && sourceSqlText.IndexOf("TOP", StringComparison.OrdinalIgnoreCase) == -1)
      {
        var i = sourceSqlText.IndexOf(Select, StringComparison.OrdinalIgnoreCase) + Select.Length;

        sourceSqlText = sourceSqlText.Insert(i, strTop);
      }

      var sqlQuery = $"{selector}{sourceSqlText}";

      var dt = _genericDatabaseQueries.GetRowData(sqlQuery);

      return dt;
    }
  }
}
