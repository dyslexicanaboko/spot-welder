using SpotWelder.Lib.Models;

namespace SpotWelder.Lib.Services
{
  public interface ITableQueryFormatService
  {
    TableQuery ParseTableName(string tableNameQuery);

    string GetClassName(TableQuery tableQuery);

    string FormatTableQuery(
      string tableQuery,
      TableQueryQualifiers qualifiers = TableQueryQualifiers.Schema | TableQueryQualifiers.Table);

    string FormatTableQuery(
      TableQuery tableQuery,
      TableQueryQualifiers qualifiers = TableQueryQualifiers.Schema | TableQueryQualifiers.Table);
  }
}
