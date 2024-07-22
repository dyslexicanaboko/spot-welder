using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.CodeFactory;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class ClassInterfaceGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.GenerateInterface;

    protected override string TemplateName => "Interface.cs.template";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      instructions.ClassName = instructions.InterfaceName;

      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      template.Replace("{{Namespace}}", instructions.Namespace);
      template.Replace("{{ClassName}}", instructions.ClassName);
      template.Replace("{{Namespaces}}", FormatNamespaces(instructions.Namespaces));
      template.Replace("{{Properties}}", FormatProperties(instructions.Properties));

      return GetFormattedCSharpResult($"{instructions.ClassName}.cs", template);
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
