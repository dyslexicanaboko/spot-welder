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
    /// Namespace used for all classes that are generated
    /// </summary>
    public string Namespace { get; set; }

    /// <summary>
    /// Name of the target subject for generation. The subject can be a source class, table or query.
    /// Additionally, this name does not have a prefix or suffix such as `Entity`, `Model`, `Dto`, etc.
    /// In other words, it's JUST the name of the subject.
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
