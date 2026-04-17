using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using SpotWelder.Lib.Services.CodeFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpotWelder.Lib.Services.Generators;

/* Everything related to formatting code, formatting text, manipulating strings for the purposes of end result generation.
 *  Formatting code
 *  Manipulating strings.
 *  Cleaning up text. */
public partial class GeneratorBase
{
  protected virtual string GetTextBlock<T>(List<T> items, Func<T, string> formatting, string? separator = null)
  {
    if (items.Count == 0) return string.Empty;

    separator ??= Environment.NewLine;

    return string.Join(separator, items.Select(formatting));
  }

  protected virtual string FormatClassAttributes(List<string> classAttributes)
    => GetTextBlock(classAttributes, ca => $"[{ca}]");

  protected virtual string FormatUsingDirectives(HashSet<string> usingDirectives)
    => GetTextBlock(usingDirectives.OrderBy(x => x).ToList(), ns => $"using {ns};");

  protected virtual string FormatInterface(string interfaceName)
  {
    if (string.IsNullOrWhiteSpace(interfaceName)) return string.Empty;

    //This is showing on one line for now. In the future I might format it properly on the next line.
    return " : " + interfaceName;
  }

  /// <summary>
  /// Formatting properties for a class.
  /// </summary>
  /// <param name="properties">Input properties</param>
  /// <returns>Formatted properties as one text block</returns>
  protected virtual string FormatProperties(List<ClassMemberStrings> properties)
    => GetTextBlock(
      properties,
      p => $"public {p.SystemTypeAlias} {p.Property} {{ get; set; }}",
      Environment.NewLine + Environment.NewLine);

  /// <summary>
  /// Formatting properties for a class.
  /// </summary>
  /// <param name="properties">Input properties</param>
  /// <returns>Formatted properties as one text block</returns>
  protected virtual string FormatPropertiesForValidation(List<ClassMemberStrings> properties)
    => GetTextBlock(
      properties,
      p => $"RuleFor(r => r.{p.Property});",
      Environment.NewLine + Environment.NewLine);

  /// <summary>
  /// Formatting properties for a record class.
  /// </summary>
  /// <param name="properties">Input properties</param>
  /// <returns>Formatted properties as one text block</returns>
  protected virtual string FormatPropertiesForRecord(List<ClassMemberStrings> properties)
    => GetTextBlock(
      properties,
      p =>
      {
        var kwRequired = p.IsNullable ? string.Empty : " required";

        return $"public{kwRequired} {p.SystemTypeAlias} {p.Property} {{ get; init; }}";
      },
      Environment.NewLine + Environment.NewLine);

  protected static string GetNotImplementedException(string? exceptionMessage = null)
  {
    exceptionMessage = exceptionMessage == null ? string.Empty : $"\"{exceptionMessage}\"";

    return $"throw new NotImplementedException({exceptionMessage});";
  }

  protected string FormatConstructorBody(
    List<ClassMemberStrings> properties,
    string from)
    => GetTextBlock(
      properties,
      p => $"{p.Property} = {from}.{p.Property};",
      Environment.NewLine);

  protected string FormatObjectInitializerBody(
    List<ClassMemberStrings> properties,
    string from)
    => GetTextBlock(
      properties,
      p => $"{p.Property} = {from}.{p.Property},",
      Environment.NewLine);

  /// <summary>
  /// Generates a constructor definition by substituting specified parameter values into a predefined template.
  /// </summary>
  /// <remarks>This method uses a template file to create constructor definitions dynamically. The template
  /// must contain placeholders for parameter type, parameter name, and constructor body, which are replaced with the
  /// provided values.</remarks>
  /// <param name="parameterType">The type of the parameter to be included in the generated constructor template. This value is inserted into the
  /// template at the designated parameter type placeholder.</param>
  /// <param name="parameterName">The name of the parameter to be included in the generated constructor template. This value is inserted into the
  /// template at the designated parameter name placeholder.</param>
  /// <param name="constructorBody">The body of the constructor, which will be inserted into the template at the constructor body placeholder. This
  /// typically contains initialization logic or assignments.</param>
  /// <param name="className">The name of the class for which the constructor is being generated. This value is inserted into the template at the
  /// designated class name placeholder.</param>
  /// <returns>A string containing the completed constructor definition with the specified parameter values substituted into
  /// the template.</returns>
  protected string ConstructorTemplate(
    string parameterType,
    string parameterName,
    string constructorBody,
    string className)
    //This is setting up a constructor template with tags for replacement
    => new StringBuilder(GetTemplate("ClassConstructor.cs.template"))
      .Replace("[ParameterType]", parameterType)
      .Replace("[ParameterName]", parameterName)
      .Replace("[ConstructorBody]", constructorBody)
      .Replace("[ClassName]", className)
      .ToString();

  protected static string FormatCSharp(string code)
  {
    // Parse the code into a SyntaxTree
    var syntaxTree = CSharpSyntaxTree.ParseText(code);

    // Create a workspace
    var workspace = new AdhocWorkspace();

    //TODO: Make this user configurable
    //Set formatting options for 2-space indentation
    var options = workspace.Options
      .WithChangedOption(FormattingOptions.UseTabs, LanguageNames.CSharp, false)
      .WithChangedOption(FormattingOptions.TabSize, LanguageNames.CSharp, 2)
      .WithChangedOption(FormattingOptions.IndentationSize, LanguageNames.CSharp, 2);

    // Format the syntax tree
    var formattedRoot = Formatter.Format(syntaxTree.GetRoot(), workspace, options);

    var formattedCode = formattedRoot.ToFullString();

    //TODO: These blank line detections aren't working correctly, so commenting it out for now
    
    // Remove blank lines immediately after opening braces
    //formattedCode = ReRemoveBlankLinesBeforeBraces().Replace(formattedCode, "$1");

    // Remove blank lines immediately before closing braces
    // formattedCode = ReRemoveBlankLinesAfterBraces().Replace(formattedCode, "$1$3");

    // Convert the formatted syntax tree back to a string
    return formattedCode.Trim();
  }

  /// <summary>
  /// Formats the specified string so that each line is trimmed and indented by a given number of spaces.
  /// Does not include the opening and closing triple quotes.
  /// </summary>
  /// <param name="content">The multi-line string to format. Each line will be trimmed and indented.</param>
  /// <param name="spacesIndented">The number of spaces to add to the beginning of each trimmed line. Must be zero or greater.</param>
  /// <returns>A new string consisting of the trimmed and indented lines, joined by line breaks.</returns>
  protected static string FormatAsRawString(string content, int spacesIndented)
    => string.Join(Environment.NewLine, content
      .Split([Environment.NewLine], StringSplitOptions.None | StringSplitOptions.RemoveEmptyEntries)
      .Select(x => x.Trim())
      .Select(x => x.PadLeft(x.Length + spacesIndented, ' ')));

  protected virtual string RemoveBlankLines(string content)
    => ReBlankLines().Replace(content, string.Empty);

  protected virtual string RemoveExcessBlankSpace(string content)
    => ReBlankSpace().Replace(content, string.Empty);
}
