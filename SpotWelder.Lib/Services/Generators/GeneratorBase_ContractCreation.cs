using SpotWelder.Lib.Models;
using System;
using System.Linq;

namespace SpotWelder.Lib.Services.Generators;

/* Everything related to extracting contracts and then producing them dynamically.
 *  Extracting contracts from classes.
 *  Producing matching interfaces for those same classes.  */
public partial class GeneratorBase
{
  /// <summary>
  /// Generates an interface file from a class by extracting its public method contracts.
  /// </summary>
  /// <param name="instructions">The same instructions used to by the main generation.</param>
  /// <param name="classResult">The generated class result from which public method contracts will be extracted.</param>
  /// <param name="templateName">The name of the template file to use for generating the interface.</param>
  /// <returns>A <see cref="GeneratedResult"/> containing the formatted interface code.</returns>
  protected virtual GeneratedResult GenerateInterface(
    ClassInstructions instructions,
    GeneratedResult classResult,
    string templateName)
  {
    var template = GetTemplateAsStringBuilder(templateName);

    SetAbsoluteContainingNamespace(template, instructions.ArchitectureStrategy, out var containingNamespace);

    if (templateName == "IMapper.cs.template")
    {
      SetUsingDirectivesDynamic(
        template,
        instructions.ArchitectureStrategy,
        instructions.UsingDirectives,
        instructions.Elections,
        templateName);
    }
    else
    {
      SetUsingDirectivesStatic(
        template,
        instructions.ArchitectureStrategy,
        instructions.UsingDirectives,
        templateName);
    }

    template.Replace("{{SubjectName}}", instructions.SubjectName);
    template.Replace("{{Contracts}}", ExtractCSharpClassContracts(classResult.Contents));

    //Reminder: The containing namespace is set at the generator level.
    return GetFormattedCSharpResult($"I{classResult.Filename}", template, containingNamespace);
  }

  /// <summary>
  /// Attempt one to generically and lazily extract contracts from provided C# class contents.
  /// I am doing my best to put low effort into this because I don't want to write
  /// more code than I have to. The formula never changes. The bias used here is
  /// to identify public methods, remove the `public` keyword, and finally add
  /// a semicolon at the end.
  /// </summary>
  /// <param name="contents">Generated class contents where contracts will be extracted from using RegEx.</param>
  /// <returns>Extracted contracts as a string block.</returns>
  protected static string ExtractCSharpClassContracts(string contents)
  {
    var lines = contents
      .Split([Environment.NewLine], StringSplitOptions.None)
      .Select(x => x.Trim())
      .Where(x => ReContracts().IsMatch(x))
      .Select(x => RePublic().Replace(x, string.Empty) + ";");

    return string.Join(Environment.NewLine + Environment.NewLine, lines);
  }
}