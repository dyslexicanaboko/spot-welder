using System.Collections.Generic;
using System.Text;

namespace SpotWelder.Lib.Models
{
  public class GeneratedResult(GenerationElections election, string fileName, string contents, string containingNamespace = "")
  {
    public GeneratedResult(GenerationElections election, string fileName, StringBuilder contents, string containingNamespace = "")
      : this(election, fileName, contents.ToString(), containingNamespace)
    {
      
    }

    /// <summary>
    /// Currently used for unit and integration testing.
    /// </summary>
    public GenerationElections Election { get; set; } = election;

    /// <summary>Optional namespace for this file. Can be used as the name for its containing folder.</summary>
    public string ContainingNamespace { get; set; } = containingNamespace;
    
    /// <summary>
    /// Filename only, no path.
    /// </summary>
    public string Filename { get; set; } = fileName;

    /// <summary>
    /// File contents.
    /// </summary>
    public string Contents { get; set; } = contents;

    /// <summary>
    /// Populated when the generated subject has an optional interface to use for dependency injection.
    /// </summary>
    public GeneratedResult? CorrespondingInterface { get; set; }

    //TODO: This is gross, I don't know that I will keep this the way it is.
    /// <summary>
    /// Populated when there is a heap of results to return.
    /// </summary>
    public List<GeneratedResult>? Heap { get; set; }
  }
}
