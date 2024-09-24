namespace SpotWelder.Lib.Models
{
  /// <summary>
  /// SQL Engine, with connection string and source SQL text to execute.
  /// </summary>
  public class ServerConnection
  {
    /// <summary>
    ///  The SQL Engine to use for executing the provided SQL
    /// </summary>
    public SqlEngine SqlEngine { get; set; } = SqlEngine.SqlServer;

    /// <summary>
    ///   Connection string to use for executing the provided SQL
    /// </summary>
    public string ConnectionString { get; set; } = "Data Source=.;Database=master;Integrated Security=SSPI;";

    /// <summary>
    ///   Determines if the Source SQL Text contains just a table name or a full SQL Query
    /// </summary>
    public SourceSqlType SourceSqlType { get; set; } = SourceSqlType.TableName;

    /// <summary>
    ///   Can be just the name of a table or a full SQL query
    /// </summary>
    public string SourceSqlText { get; set; } = "SELECT 1";

    /// <summary>
    ///   Structured object that formats the incoming Source SQL Text
    /// </summary>
    public TableQuery TableQuery { get; set; } = new();
  }
}
