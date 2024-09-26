using Npgsql;
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
  }
}
