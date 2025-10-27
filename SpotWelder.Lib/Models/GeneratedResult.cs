using System.Text;

namespace SpotWelder.Lib.Models
{
  public class GeneratedResult
  {
    public GeneratedResult(GenerationElections election, string fileName, StringBuilder contents)
      : this(election, fileName, contents.ToString())
    {
      
    }

    public GeneratedResult(GenerationElections election, string fileName, string contents)
    {
      Election = election;
      Filename = fileName;
      Contents = contents;
    }

    public GenerationElections Election { get; set; } = GenerationElections.None;

    public string Filename { get; set; }

    public string Contents { get; set; } = string.Empty;
  }
}
