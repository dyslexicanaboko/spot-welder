using System.Text.RegularExpressions;

namespace SpotWelder.Lib.Services.Generators;

/* Regular expressions */
public abstract partial class GeneratorBase
{
  [GeneratedRegex(@"^\s+$[\r\n]*", RegexOptions.Multiline)]
  protected static partial Regex ReBlankLines();

  [GeneratedRegex(@"^\s+$^[\r\n]", RegexOptions.Multiline)]
  protected static partial Regex ReBlankSpace();

  [GeneratedRegex(@"^public .+ .+\(.*\)$")]
  protected static partial Regex ReContracts();

  [GeneratedRegex("^public (async )?")]
  protected static partial Regex RePublic();

  // Generated with copilot
  // Remove blank lines immediately before closing braces
  [GeneratedRegex(@"(\r?\n)([ \t]*\r?\n)+([ \t]*\})")]
  protected static partial Regex ReRemoveBlankLinesBeforeBraces();

  // Generated with copilot
  // Remove blank lines immediately after opening braces
  [GeneratedRegex(@"(\{[ \t]*\r?\n)([ \t]*\r?\n)+")]
  protected static partial Regex ReRemoveBlankLinesAfterBraces();
}
