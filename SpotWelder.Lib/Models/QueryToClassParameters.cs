namespace SpotWelder.Lib.Models
{
  public class QueryToClassParameters
  {
    /// <summary>
    /// SQL Engine, with connection string and source SQL text to execute.
    /// </summary>
    public ServerConnection ServerConnection { get; set; } = new();

    /// <summary>
    /// This may be phased out, offers the option of C# versus VB.Net but this may not matter anymore because this project is
    /// moving towards template based generation. If someone wants to keep using inferior VB.Net they can put in the work to
    /// make a shitty template for it.
    /// </summary>
    public CodeType LanguageType { get; set; }

    /// <summary>
    /// The way in which class files are organized physically. There are two types of organization currently: flat and layered.
    ///   N-Tier architecture means class files are generated in separate folders based on their type, such as `Entities`, `Models`, `Dtos`, etc.
    ///   Feature-based development architecture means all class files are generated in a single folder.
    ///
    /// Besides the physical file organization, the namespaces have to be adjusted to match.
    /// </summary>
    public ArchitectureType ArchitectureType { get; set; }

    /// <summary>
    /// Root namespace used for all classes that are generated. All architecture is appended to this root.
    /// </summary>
    public string RootContainingNamespace { get; set; }

    /// <summary>
    /// Name of the target subject for generation. The subject can be a source class, table or query.
    /// Additionally, this name does not have a prefix or suffix such as `Entity`, `Model`, `Dto`, etc.
    /// In other words, it's JUST the name of the subject.
    /// All other names are derived from this name using the formula of: "[SubjectName] + [Suffix]".
    /// </summary>
    /// <example> Table named: `dbo.Task`, the subject would just be `Task`.</example>
    public string SubjectName { get; set; }
    
    /// <summary> The user's elections. In other words, what should be generated. </summary>
    public GenerationElections Elections { get; set; } = GenerationElections.None;

    /// <summary> Check if any elections were made. </summary>
    //If this is zero, then no elections were made. Therefore, if it's not zero, positive or negative, it has elections.
    public bool HasElections => Elections != GenerationElections.None;
  }
}
