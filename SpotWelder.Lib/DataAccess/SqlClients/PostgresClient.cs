using Npgsql;
using System;
using System.Data;

namespace SpotWelder.Lib.DataAccess.SqlClients
{
  /// <summary> Abstracted interface for Postgres client </summary>
  public class PostgresClient : BaseSqlClient
  {
    /// <inheritdoc />
    public override IDbConnection GetDbConnection(string connectionString) => new NpgsqlConnection(connectionString);

    /// <inheritdoc />
    public override IDbCommand GetDbCommand(string cmdText, IDbConnection connection)
      => new NpgsqlCommand(cmdText, (NpgsqlConnection)connection);

    /// <inheritdoc />
    public override IDbDataAdapter GetDbDataAdapter(IDbCommand selectCommand) => new NpgsqlDataAdapter((NpgsqlCommand)selectCommand);

    /// <inheritdoc />
    public override string GetSchemaQuery(SourceSqlType sourceSqlType, string sourceSqlText)
    {
      if (sourceSqlType != SourceSqlType.Query) return $"SELECT * FROM {sourceSqlText} LIMIT 0";

      if(!sourceSqlText.Contains("LIMIT", StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Queries must end with `LIMIT 0` clause. 0x202410270025");

      return sourceSqlText;
    }
  }
}
