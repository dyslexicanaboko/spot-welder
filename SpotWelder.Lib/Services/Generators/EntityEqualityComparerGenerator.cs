using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.CodeFactory;
using System;
using System.Collections.Generic;

namespace SpotWelder.Lib.Services.Generators
{
  public class EntityEqualityComparerGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.EntityEqualityComparer;

    protected override string TemplateName => "EntityEqualityComparer.cs.template";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      var template = GetTemplateAsStringBuilder(TemplateName);

      SetAbsoluteContainingNamespace(template, instructions.ArchitectureStrategy, out var containingNamespace);

      SetUsingDirectivesStatic(
        template,
        instructions.ArchitectureStrategy,
        instructions.UsingDirectives);

      template.Replace("{{EntityName}}", instructions.EntityName);
      template.Replace("{{PropertiesEquals}}", FormatForEquals(instructions.Properties));
      template.Replace("{{PropertiesHashCode}}", FormatForHashCode(instructions.Properties));

      return GetFormattedCSharpResult($"{instructions.EntityName}EqualityComparer.cs", template, containingNamespace);
    }

    private string FormatForEquals(List<ClassMemberStrings> properties)
      => GetTextBlock(
        properties,
        p => $"        left.{p.Property} == right.{p.Property}",
        " && " + Environment.NewLine);

    private string FormatForHashCode(List<ClassMemberStrings> properties)
      => GetTextBlock(
        properties,
        p => $"        obj.{p.Property}.GetHashCode()",
        " + " + Environment.NewLine);
  }
}
