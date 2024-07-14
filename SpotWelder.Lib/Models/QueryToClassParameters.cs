namespace SpotWelder.Lib.Models
{
  public class QueryToClassParameters
  {
    /// <summary>
    ///   Connection string to use for executing the provided SQL
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    ///   This may be phased out, offers the option of C# versus VB.Net but this may not matter anymore because this project is
    ///   moving towards
    ///   template based generation. If someone wants to keep using inferior VB.Net they can put in the work to make a shitty
    ///   template for it.
    /// </summary>
    public CodeType LanguageType { get; set; }

    /// <summary>
    ///   Determines if the Source SQL Text contains just a table name or a full SQL Query
    /// </summary>
    public SourceSqlType SourceSqlType { get; set; }

    /// <summary>
    ///   Can be just the name of a table or a full SQL query
    /// </summary>
    public string SourceSqlText { get; set; }

    /// <summary>
    ///   Structured object that formats the incoming Source SQL Text
    /// </summary>
    public TableQuery TableQuery { get; set; } = new();

    /// <summary>
    ///   File name only
    /// </summary>
    public string Filename { get; set; }

    /// <summary>
    ///   Path where file will be saved
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    ///   Save generated output as a file
    /// </summary>
    public bool SaveAsFile { get; set; }

    /// <summary>
    ///   Overwrite existing files that may have the same name
    /// </summary>
    public bool OverwriteExistingFiles { get; set; }

    /// <summary>
    ///   Namespace used for all classes that are generated
    /// </summary>
    public string Namespace { get; set; }

    /// <summary>
    /// Name of the target subject for generation. The subject can be a source class, table or query.
    /// Additionally, this name does not have a prefix or suffix such as `Entity`, `Model`, `Dto`, etc.
    /// In other words, it's JUST the name of the subject.
    /// </summary>
    /// <example> Table named: `dbo.Task`, the subject would just be `Task`.</example>
    public string SubjectName { get; set; }

    /// <summary> Name of the subject with the `Entity` suffix.</summary>
    /// <example> Subject named: `Task`, the entity would be `TaskEntity`.</example>
    public string EntityName { get; set; }

    /// <summary> Name of the subject with the `Model` suffix.</summary>
    /// <example> Subject named: `Task`, the entity would be `TaskModel`.</example>
    public string ModelName { get; set; }

    /// <summary> Name of the subject in camelCase and pluralized.</summary>
    /// <example> Subject named: `Task`, the API Route would be `tasks` as in `api/v1/tasks`.</example>
    public string ApiRoute { get; set; }

    //Future: If there are too many options, then use a dictionary of data, like <string, bool> encapsulated in a class
    /// <summary> The user's elections. In other words, what should be generated. </summary>
    public GenerationElections Elections { get; set; } = GenerationElections.None;

    public bool HasElections => Elections > 0;
  }
}
