using System.Text;

namespace SpotWelder.Lib.Models
{
  public class GeneratedResult
  {
    public GeneratedResult(string fileName) => Filename = fileName;

    public GeneratedResult(string fileName, StringBuilder contents)
    {
      Filename = fileName;
      Contents = contents.ToString();
    }

    public GeneratedResult(string fileName, string contents)
    {
      Filename = fileName;
      Contents = contents;
    }

    public string Filename { get; set; }

    public string Contents { get; set; } = string.Empty;
  }
}
