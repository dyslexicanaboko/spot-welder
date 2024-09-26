using SpotWelder.Lib.Models;

namespace SpotWelder.Lib.Services.TableQueryFormats
{
  [ExcludeFromDiScan]
  public interface ITableQueryFormatStrategy
  {
    SqlEngine SqlEngine { get; }

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
