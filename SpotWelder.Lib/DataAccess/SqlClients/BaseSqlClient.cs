using System.Data;

namespace SpotWelder.Lib.DataAccess.SqlClients;

public abstract class BaseSqlClient
{
  /// <summary> The type of SQL engine this client is connected to. </summary>
  public abstract SqlEngine SqlEngine { get; }

  public abstract IDbConnection GetDbConnection(string connectionString);

  public abstract IDbCommand GetDbCommand(string cmdText, IDbConnection connection);

  public abstract IDbDataAdapter GetDbDataAdapter(IDbCommand selectCommand);

  /// <summary>
  /// In order to get the schema of a table or query only, there is a specific type of
  /// query that is executed depending on the SQL engine.
  /// </summary>
  /// <param name="sourceSqlType">What kind of SQL query is it? Table name only? Or full query?</param>
  /// <param name="sourceSqlText">Table name or raw query</param>
  /// <returns>Schema query formatted for the source Server Connection.</returns>
  public abstract string GetSchemaQuery(SourceSqlType sourceSqlType, string sourceSqlText);
}
