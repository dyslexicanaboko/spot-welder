using System.Collections.Generic;
using SpotWelder.Lib.Services.CodeFactory;

namespace SpotWelder.Lib.Models
{
  public class ClassInstructions
  {
    /// <summary>
    /// Table query for the class being generated. This may not be in use depending
    /// on the pathing being used.
    /// </summary>
    public TableQuery TableQuery { get; set; }

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
    
    /// <summary>Namespaces that the class being generated should be using (importing).</summary>
    public IList<string> Namespaces { get; set; } = new List<string>();

    /// <summary>Class attributes</summary>
    public IList<string> ClassAttributes { get; set; } = new List<string>();

    /// <summary>Properties of the source entity.</summary>
    public IList<ClassMemberStrings> Properties { get; set; } = new List<ClassMemberStrings>();

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
        EntityName = EntityName,
        ModelName = ModelName,
        Namespace = Namespace,
        InterfaceName = InterfaceName,
        ApiRoute = ApiRoute,
        TableQuery = TableQuery.Clone()
      };

      c.ClassAttributes = new List<string>(ClassAttributes);
      c.Namespaces = new List<string>(Namespaces);

      foreach (var p in Properties) c.Properties.Add(p.Clone());

      return c;
    }
  }
}
