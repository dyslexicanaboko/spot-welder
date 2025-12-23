using System.Text;

namespace SpotWelder.Lib.Models
{
  public class GeneratedResult(GenerationElections election, string fileName, string contents)
  {
    public GeneratedResult(GenerationElections election, string fileName, StringBuilder contents)
      : this(election, fileName, contents.ToString())
    {
      
    }

    /// <summary>
    /// Currently used for unit and integration testing.
    /// </summary>
    public GenerationElections Election { get; set; } = election;

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
  }
}
