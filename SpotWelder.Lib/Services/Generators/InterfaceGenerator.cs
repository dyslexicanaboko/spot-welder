using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.CodeFactory;
using System;
using System.Collections.Generic;

namespace SpotWelder.Lib.Services.Generators
{
  //TODO: Not sure if I am going to keep this anymore. Has not been practical.
  [Obsolete("Not sure if I am going to keep this anymore. Has not been practical.")]
  public class InterfaceGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.Interface;

    protected override string TemplateName => "Interface.cs.template";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      instructions.ClassName = instructions.InterfaceName;

      var template = GetTemplateAsStringBuilder(TemplateName);

      template.Replace("{{RootContainingNamespace}}", instructions.RootContainingNamespace);
      template.Replace("{{ClassName}}", instructions.ClassName);
      template.Replace("{{UsingDirectives}}", FormatUsingDirectives(instructions.UsingDirectives));
      template.Replace("{{Properties}}", FormatProperties(instructions.Properties));

      return GetFormattedCSharpResult($"{instructions.ClassName}.cs", template, string.Empty);
    }

    protected override string FormatProperties(IList<ClassMemberStrings> properties)
    {
      var content = GetTextBlock(
        properties,
        p => $"        {p.SystemTypeAlias} {p.Property} {{ get; set; }}",
        Environment.NewLine + Environment.NewLine);

      return content;
    }
  }
}
