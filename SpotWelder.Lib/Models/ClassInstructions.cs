using SpotWelder.Lib.Services.CodeFactory;
using SpotWelder.Lib.Services.CodeFactory.AsynchronicityStrategy;
using System;
using System.Collections.Generic;

namespace SpotWelder.Lib.Models
{
  //NOTE: When adding new properties, be sure to add them to the Clone method below too.
  public class ClassInstructions
  {
    public SqlEngine SqlEngine { get; set; }

    /// <summary>
    /// The original, unmodified, query provided by the user.
    /// Use to determine if the generators will produce readonly pipelines or full CRUD.
    /// When a <see cref="Lib.SourceSqlType.Query"/> is provided then CUD cannot be reliably produced.
    /// When a <see cref="Lib.SourceSqlType.TableName"/> is provided then all CRUD can be produced so long as there is a primary key.
    /// </summary>
    public SourceSqlType SourceSqlType { get; set; }

    /// <summary>
    /// The way in which class files are organized physically. There are two types of organization currently: flat and layered.
    ///   N-Tier architecture means class files are generated in separate folders based on their type, such as `Entities`, `Models`, `Dtos`, etc.
    ///   Feature-based development architecture means all class files are generated in a single folder.
    ///
    /// Besides the physical file organization, the namespaces have to be adjusted to match.
    /// </summary>
    public ArchitectureType ArchitectureType { get; set; }

    /// <summary>
    /// Table query for the class being generated. This may not be in use depending
    /// on the pathing being used.
    /// </summary>
    public TableQuery TableQuery { get; set; }

    /// <summary>
    /// The original query provided by the user. This is used when a solitary table isn't the target.
    /// When the user provides a compound query, such as an inner join between two tables, this is
    /// where that query is stored.
    /// </summary>
    public string SourceQuery { get; set; }

    /// <summary>
    /// Name of the class being generated. This is a property that is dedicated to being
    /// the name of whatever is being generated regardless of its purpose.
    /// </summary>
    public string ClassName { get; set; }

    /// <summary>
    ///   Name of the target subject for generation. The subject can be a source class, table or query.
    ///   Additionally, this name does not have a prefix or suffix such as `Entity`, `Model`, `Dto`, etc.
    ///   In other words, it's JUST the name of the subject.
    /// </summary>
    /// <example> Table named: `dbo.Task`, the subject would just be `Task`.</example>
    public string SubjectName { get; set; }

    /// <summary> Name of the subject with the `Entity` suffix.</summary>
    /// <example> Subject named: `Task`, the entity would be `TaskEntity`.</example>
    public string EntityName { get; set; }

    /// <summary> Name of the subject with the `Record` suffix.</summary>
    /// <example> Subject named: `Task`, the record would be `TaskRecord`.</example>
    public string RecordName { get; set; }

    /// <summary> Name of the subject with the `Model` suffix.</summary>
    /// <example> Subject named: `Task`, the entity would be `TaskModel`.</example>
    public string ModelName { get; set; }

    /// <summary> Name of the subject in camelCase and pluralized.</summary>
    /// <example> Subject named: `Task`, the API Route would be `tasks` as in `api/v1/tasks`.</example>
    public string ApiRoute { get; set; }

    /// <summary>Namespace used for all classes. It's just a container for the code and not intended for use.</summary>
    public string Namespace { get; set; }

    /// <summary>
    /// Name of the subject with the `I` prefix. Single interface name for now.
    /// May change it to be a list in the future, but I can't think of a reason
    /// as to why that would be warranted right now.
    /// </summary>
    /// <example> Subject named: `Task`, the interface would be `ITask`.</example>
    public string InterfaceName { get; set; }

    /// <summary> Should class be a partial class? </summary>
    public bool IsPartial { get; set; }

    /// <summary> Should class methods be asynchronous? </summary>
    public bool IsAsynchronous { get; set; }

    /// <summary>Namespaces that the class being generated should be using (importing).</summary>
    public IList<string> Namespaces { get; set; } = new List<string>();

    /// <summary>Class attributes</summary>
    public IList<string> ClassAttributes { get; set; } = new List<string>();

    /// <summary>
    /// This is a temporary concept until I can do better. The different languages to
    /// generate something in. Vague explanation for a vague implementation.
    /// </summary>
    public CodeType Languages { get; set; }
    
    /// <summary> The user's elections. In other words, what should be generated. </summary>
    public GenerationElections Elections { get; set; } = GenerationElections.None;

    /// <summary>Properties of the source entity.</summary>
    public IList<ClassMemberStrings> Properties { get; set; } = new List<ClassMemberStrings>();

    /// <summary>Formatter for formatting asynchronous syntax if elected. Otherwise, formatted as synchronous syntax.</summary>
    public AsynchronicityFormatStrategyBase AsynchronicityFormatStrategy { get; set; }

    //TODO: Not sure if I need this anymore
    //The intention here was to be able to add a namespace that is needed in every class,
    //but with global namespaces now available this is obsolete
    [Obsolete("Global namespaces makes this unnecessary.")]
    public void AddNamespace(string nameSpace)
    {
      if (Namespaces.Contains(nameSpace)) return;

      Namespaces.Add(nameSpace);
    }

    public ClassInstructions Clone()
    {
      var c = new ClassInstructions
      {
        SubjectName = SubjectName,
        RecordName = RecordName,
        EntityName = EntityName,
        ModelName = ModelName,
        Namespace = Namespace,
        InterfaceName = InterfaceName,
        ApiRoute = ApiRoute,
        IsAsynchronous = IsAsynchronous,
        Languages = Languages,
        Elections = Elections,
        SqlEngine = SqlEngine,
        SourceQuery = SourceQuery,
        SourceSqlType = SourceSqlType,
        ArchitectureType = ArchitectureType,
        TableQuery = TableQuery.Clone(),
        AsynchronicityFormatStrategy = AsynchronicityFormatStrategy.Clone(),
        ClassAttributes = new List<string>(ClassAttributes),
        Namespaces = new List<string>(Namespaces)
      };

      foreach (var p in Properties) c.Properties.Add(p.Clone());

      return c;
    }
  }
}
