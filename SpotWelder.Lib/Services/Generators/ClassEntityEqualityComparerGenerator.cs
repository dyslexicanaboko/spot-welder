using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.CodeFactory;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class ClassEntityEqualityComparerGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.EntityEqualityComparer;

    protected override string TemplateName => "EntityEqualityComparer.cs.template";

    /// <inheritdoc />
    protected override string ContainingNamespace => "Entities";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      instructions.ClassName = instructions.EntityName;
      
      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      SetContainingNamespace(template);
      template.Replace("{{Namespace}}", instructions.RootContainingNamespace);
      template.Replace("{{ClassName}}", instructions.ClassName);
      template.Replace("{{EntityName}}", instructions.EntityName);
      template.Replace("{{Namespaces}}", FormatUsingDirectives(instructions.UsingDirectives));

      //Method bodies
      template.Replace("{{PropertiesEquals}}", FormatForEquals(instructions.Properties));
      template.Replace("{{PropertiesHashCode}}", FormatForHashCode(instructions.Properties));

      return GetFormattedCSharpResult($"{instructions.EntityName}EqualityComparer.cs", template);
    }

    private string FormatForEquals(IList<ClassMemberStrings> properties)
    {
      var content = GetTextBlock(
        properties,
        p => $"        left.{p.Property} == right.{p.Property}",
        " && " + Environment.NewLine);

      return content;
    }

    private string FormatForHashCode(IList<ClassMemberStrings> properties)
    {
      var content = GetTextBlock(
        properties,
        p => $"        obj.{p.Property}.GetHashCode()",
        " + " + Environment.NewLine);

      return content;
    }
  }
}
