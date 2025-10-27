namespace SpotWelder.Lib.Models
{
  public class QueryToMockDataParameters
  {
    /// <summary>
    ///   SQL Engine, with connection string and source SQL text to execute.
    /// </summary>
    public ServerConnection ServerConnection { get; set; } = new();

    public string ClassEntityName { get; set; }
  }
}
