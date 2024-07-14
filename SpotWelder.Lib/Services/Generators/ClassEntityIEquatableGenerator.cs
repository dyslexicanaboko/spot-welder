using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.CodeFactory;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class ClassEntityIEquatableGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.GenerateEntityIEquatable;

    protected override string TemplateName => "EntityIEquatable.cs.template";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      instructions.ClassName = instructions.EntityName;

      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      template.Replace("{{Namespace}}", instructions.Namespace);
      template.Replace("{{ClassName}}", instructions.ClassName);
      template.Replace("{{EntityName}}", instructions.EntityName);
      template.Replace("{{Namespaces}}", FormatNamespaces(instructions.Namespaces));

      var t = template.ToString();

      t = RemoveExcessBlankSpace(t);

      //t = RemoveBlankLines(t);

      t = t.Replace("{{PropertiesEquals}}", FormatForEquals(instructions.Properties));
      t = t.Replace("{{PropertiesHashCode}}", FormatForHashCode(instructions.Properties));

      return new(instructions.EntityName + "_IEquatable.cs", t);
    }

    private string FormatForEquals(IList<ClassMemberStrings> properties)
    {
      var content = GetTextBlock(
        properties,
        p => $"                {p.Property} == other.{p.Property}",
        " && " + Environment.NewLine);

      return content;
    }

    private string FormatForHashCode(IList<ClassMemberStrings> properties)
    {
      var content = GetTextBlock(
        properties,
        p => $"                {p.Property}.GetHashCode()",
        " + " + Environment.NewLine);

      return content;
    }
  }
}
