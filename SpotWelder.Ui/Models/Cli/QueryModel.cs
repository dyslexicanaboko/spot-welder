using SpotWelder.Lib;
using YamlDotNet.Serialization;

namespace SpotWelder.Ui.Models.Cli
{
  public class QueryModel
  {
    [YamlMember(Alias = "subjectName")]
    public string SubjectName { get; set; }
   
    /// <summary> Determines if the Source SQL Text contains just a table name or a full SQL Query. </summary>
    [YamlMember(Alias = "sourceSqlType")]
    public SourceSqlType SourceSqlType { get; set; }

    /// <summary> Can be just the name of a table or a full SQL query. </summary>
    [YamlMember(Alias = "sourceSqlText")]
    public string SourceSqlText { get; set; }
  }
}
