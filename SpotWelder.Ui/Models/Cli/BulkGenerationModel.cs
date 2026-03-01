using SpotWelder.Lib;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace SpotWelder.Ui.Models.Cli
{
  public class BulkGenerationModel
  {
    /// <summary> The SQL Engine to use for executing the provided SQL. </summary>
    [YamlMember(Alias = "sqlEngine")]
    public SqlEngine SqlEngine { get; set; }

    /// <summary> Connection string to use for executing the provided SQL </summary>
    [YamlMember(Alias = "connectionString")]
    public string ConnectionString { get; set; }

    [YamlMember(Alias = "namespace")]
    public string Namespace { get; set; }
    
    [YamlMember(Alias = "destinationFolder")]
    public string DestinationFolder { get; set; }

    [YamlMember(Alias = "electionsList")]
    public List<string> ElectionsList { get; set; }
    
    [YamlMember(Alias = "queries")]
    public List<QueryModel> Queries { get; set; }
  }
}
