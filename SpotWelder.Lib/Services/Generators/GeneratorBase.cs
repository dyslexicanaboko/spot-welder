using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.Generators.Elections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SpotWelder.Lib.Services.Generators
{
  /* Root base class broken down into partials
   *  Contract creation
   *  Namespace handling
   *  Text formatting */
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

    protected readonly string TemplatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");

    public abstract GenerationElections Election { get; }

    protected abstract string TemplateName { get; }

    public abstract GeneratedResult FillTemplate(ClassInstructions instructions);

    protected virtual GeneratedResult GetFormattedCSharpResult(
      string fileNameWithExtension, 
      StringBuilder contents,
      string containingNamespace)
      => new(
        Election, 
        fileNameWithExtension, 
        FormatCSharp(contents.ToString()), 
        containingNamespace);

    protected virtual string GetTemplate(string templateName)
    {
      var file = Path.Combine(TemplatesPath, templateName);

      return !File.Exists(file) ? 
        throw new FileNotFoundException("Template file not found. Check the spelling and try again. Ex: ReadOnly vs. ReadsOnly", file) : 
        File.ReadAllText(file);
    }

    protected virtual StringBuilder GetTemplateAsStringBuilder(string templateName)
      => new(GetTemplate(templateName));

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
  }
}
