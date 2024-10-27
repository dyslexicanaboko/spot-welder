using Microsoft.Data.SqlClient;
using System.Data;

namespace SpotWelder.Lib.DataAccess.SqlClients
{
  /// <summary> Abstracted interface for SQL Server client </summary>
  public class SqlServerClient : BaseSqlClient
  {
    /// <inheritdoc />
    public override IDbConnection GetDbConnection(string connectionString) => new SqlConnection(connectionString);

    /// <inheritdoc />
    public override IDbCommand GetDbCommand(string cmdText, IDbConnection connection) => new SqlCommand(cmdText, (SqlConnection)connection);

    /// <inheritdoc />
    public override IDbDataAdapter GetDbDataAdapter(IDbCommand selectCommand) => new SqlDataAdapter((SqlCommand)selectCommand);

    /// <inheritdoc />
    public override string GetSchemaQuery(SourceSqlType sourceSqlType, string sourceSqlText)
    {
      var selector = sourceSqlType == SourceSqlType.TableName ? "SELECT * FROM " : string.Empty;

      return $"SET FMTONLY ON; {selector}{sourceSqlText}; SET FMTONLY OFF;";
    }
  }
}
