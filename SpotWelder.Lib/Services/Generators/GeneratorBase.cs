using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.CodeFactory;
using SpotWelder.Lib.Services.Generators.Elections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpotWelder.Lib.Services.Generators
{
  public abstract class GeneratorBase
  {
    protected readonly Regex ReBlankLines = new(@"^\s+$[\r\n]*", RegexOptions.Multiline);

    protected readonly Regex ReBlankSpace = new(@"^\s+$^[\r\n]", RegexOptions.Multiline);

    /// <summary>Matching the beginning of a contract. Excludes constructors on purpose.</summary>
    protected readonly Regex ReContracts = new (@"^public .+ .+\(.*\)$");

    /// <summary>Matching the beginning of a contract to remove these keywords.</summary>
    protected readonly Regex RePublic = new("^public (async )?");

    protected readonly string TemplatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");

    public abstract GenerationElections Election { get; }

    protected abstract string TemplateName { get; }

    /// <summary>
    /// The namespace where the generated content will reside.
    /// Can be used to name folders too.
    /// </summary>
    protected virtual string ContainingNamespace => string.Empty;

    public abstract GeneratedResult FillTemplate(ClassInstructions instructions);

    protected virtual GeneratedResult GetFormattedCSharpResult(
      string fileNameWithExtension, 
      StringBuilder contents)
      => new(Election, fileNameWithExtension, FormatCSharp(contents.ToString()), ContainingNamespace);

    protected virtual string GetTemplate(string templateName)
    {
      var file = Path.Combine(TemplatesPath, templateName);

      if(!File.Exists(file)) throw new FileNotFoundException("Template file not found. Check the spelling and try again. Ex: ReadOnly vs. ReadsOnly", file);

      var str = File.ReadAllText(file);

      return str;
    }

    protected virtual string GetTextBlock<T>(IList<T> items, Func<T, string> formatting, string separator = null)
    {
      if (items.Count == 0) return string.Empty;

      var lst = new List<string>(items.Count);

      foreach (var item in items)
      {
        var formatted = formatting(item);

        lst.Add(formatted);
      }

      if (separator == null) separator = Environment.NewLine;

      var content = string.Join(separator, lst);

      return content;
    }

    protected virtual string RemoveBlankLines(string content)
    {
      var replacement = ReBlankLines.Replace(content, string.Empty);

      return replacement;
    }

    protected virtual string RemoveExcessBlankSpace(string content)
    {
      var replacement = ReBlankSpace.Replace(content, string.Empty);

      return replacement;
    }

    protected virtual string FormatClassAttributes(IList<string> classAttributes)
    {
      var content = GetTextBlock(classAttributes, ca => $"[{ca}]");

      return content;
    }

    protected virtual string FormatNamespaces(IList<string> namespaces)
    {
      //Purposely not going to sort the namespaces just in case a specific order was wanted
      //Will remove the redundant namespaces though
      namespaces = namespaces.Distinct().ToList();

      var content = GetTextBlock(namespaces, ns => $"using {ns};");

      return content;
    }

    protected virtual string FormatInterface(string interfaceName)
    {
      if (string.IsNullOrWhiteSpace(interfaceName)) return string.Empty;

      //This is showing on one line for now. In the future I might format it properly on the next line.
      var content = " : " + interfaceName;

      return content;
    }

    /// <summary>
    /// Formatting properties for a class.
    /// </summary>
    /// <param name="properties">Input properties</param>
    /// <returns>Formatted properties as one text block</returns>
    protected virtual string FormatProperties(IList<ClassMemberStrings> properties)
    {
      var content = GetTextBlock(
        properties,
        p => $"public {p.SystemTypeAlias} {p.Property} {{ get; set; }}",
        Environment.NewLine + Environment.NewLine);

      return content;
    }

    /// <summary>
    /// Formatting properties for a record class.
    /// </summary>
    /// <param name="properties">Input properties</param>
    /// <returns>Formatted properties as one text block</returns>
    protected virtual string FormatPropertiesForRecord(IList<ClassMemberStrings> properties)
    {
      var content = GetTextBlock(
        properties,
        p =>
        {
          var kwRequired = p.IsNullable ? " required" : string.Empty;

          return $"public{kwRequired} {p.SystemTypeAlias} {p.Property} {{ get; init; }}";
        },
        Environment.NewLine + Environment.NewLine);

      return content;
    }

    protected string GetNotImplementedException(string exceptionMessage = null)
    {
      if (exceptionMessage == null)
        exceptionMessage = string.Empty;
      else
        exceptionMessage = $"\"{exceptionMessage}\"";

      return $"throw new NotImplementedException({exceptionMessage});";
    }

    protected string FormatConstructorBody(
      IList<ClassMemberStrings> properties,
      string from)
    {
      var content = GetTextBlock(
        properties,
        p => $"{p.Property} = {from}.{p.Property};",
        Environment.NewLine);

      return content;
    }

    protected string FormatObjectInitializerBody(
      IList<ClassMemberStrings> properties,
      string from)
    {
      var content = GetTextBlock(
        properties,
        p => $"{p.Property} = {from}.{p.Property},",
        Environment.NewLine);

      return content;
    }

    //TODO: Need to use DI for this
    protected AsynchronicityFormatStrategyBase GetAsynchronicityFormatStrategy(bool isAsynchronous)
    {
      AsynchronicityFormatStrategyBase strategy = isAsynchronous ? new AsyncFormatStrategy() : new SyncFormatStrategy();

      strategy.Configure();

      return strategy;
    }

    protected static List<GenerationElections> GetChildElections(
      GenerationElections elections,
      GenerationElections parent)
    {
      var lst = new List<GenerationElections>();

      elections.GetFlags().ForEach(e =>
      {
        var fi = e.GetType().GetField(e.ToString());

        if (fi == null) return;

        var attr = fi.GetCustomAttributes(false);

        if (attr.Length == 0) return;

        var child = attr[0];

        if(child is GenerationElectionChildAttribute childAttr && childAttr.Parent == parent) lst.Add(e);
      });

      return lst;
    }

    protected string ConstructorTemplate(
      string parameterType,
      string parameterName,
      string constructorBody)
    {
      var sb = new StringBuilder(GetTemplate("ClassConstructor.cs.template"));

      //This is setting up a constructor template with tags for replacement
      sb
        .Replace("[ParameterType]", parameterType)
        .Replace("[ParameterName]", parameterName)
        .Replace("[ConstructorBody]", constructorBody);

      return sb.ToString();
    }

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

      // Convert the formatted syntax tree back to a string
      var formattedCode = formattedRoot.ToFullString();

      // Output the formatted code
      return formattedCode;
    }

    protected StringBuilder SetContainingNamespace(StringBuilder sb)
      => sb.Replace("{{ContainingNamespace}}", ContainingNamespace);

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
      var template = new StringBuilder(GetTemplate(templateName));

      SetContainingNamespace(template);
      template.Replace("{{Namespaces}}", FormatNamespaces(instructions.Namespaces));
      template.Replace("{{Namespace}}", instructions.Namespace);
      template.Replace("{{ClassName}}", instructions.ClassName);
      template.Replace("{{Contracts}}", ExtractCSharpClassContracts(classResult.Contents));

      //Reminder: The containing namespace is set at the generator level.
      return GetFormattedCSharpResult($"I{classResult.Filename}", template);
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
    protected string ExtractCSharpClassContracts(string contents)
    {
      var lines = contents
        .Split([Environment.NewLine], StringSplitOptions.None)
        .Select(x => x.Trim())
        .Where(x => ReContracts.IsMatch(x))
        .Select(x => RePublic.Replace(x,string.Empty) + ";");

      return string.Join(Environment.NewLine + Environment.NewLine, lines);
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
  }
}
