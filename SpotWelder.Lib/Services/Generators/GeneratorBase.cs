using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.CodeFactory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SpotWelder.Lib.Services.Generators
{
  public abstract class GeneratorBase
  {
    protected readonly Regex _reBlankLines = new (@"^\s+$[\r\n]*", RegexOptions.Multiline);

    protected readonly Regex _reBlankSpace = new (@"^\s+$^[\r\n]", RegexOptions.Multiline);

    private readonly string _templatesPath;

    protected GeneratorBase(ClassInstructions instructions, string templateName)
    {
      _templatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");

      TemplateName = templateName;

      Instructions = instructions;
    }

    protected ClassInstructions Instructions { get; set; }

    public string TemplateName { get; protected set; }

    protected virtual GeneratedResult GetResult(string extension = "cs")
    {
      var r = new GeneratedResult { Filename = Instructions.ClassName + "." + extension };

      return r;
    }

    protected virtual string GetTemplate(string templateName)
    {
      var file = Path.Combine(_templatesPath, templateName);

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
      var replacement = _reBlankLines.Replace(content, string.Empty);

      return replacement;
    }

    protected virtual string RemoveExcessBlankSpace(string content)
    {
      var replacement = _reBlankSpace.Replace(content, string.Empty);

      return replacement;
    }

    public abstract GeneratedResult FillTemplate();

    protected virtual string FormatClassAttributes(IList<string> classAttributes)
    {
      var content = GetTextBlock(classAttributes, ca => $"[{ca}]");

      return content;
    }

    protected virtual string FormatNamespaces(IList<string> namespaces)
    {
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

    protected virtual string FormatProperties(IList<ClassMemberStrings> properties)
    {
      var content = GetTextBlock(
        properties,
        p => $"        public {p.SystemTypeAlias} {p.Property} {{ get; set; }}",
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
        p => $"			{p.Property} = {from}.{p.Property};",
        Environment.NewLine);

      return content;
    }

    //TODO: Need to use DI for this
    protected AsynchronicityFormatStrategyBase GetAsynchronicityFormatStrategy(bool isAsynchronous)
    {
      AsynchronicityFormatStrategyBase strategy = isAsynchronous
        ? new AsyncFormatStrategy()
        : new SyncFormatStrategy();
      
      strategy.Configure();

      return strategy;
    }
  }
}
