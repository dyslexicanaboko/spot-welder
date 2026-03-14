using SpotWelder.Lib.Models;
using System.Collections.Generic;
using System;

namespace SpotWelder.Lib.Services.Generators
{
  public class ClassModelGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.Model;

    protected override string TemplateName => "Model.cs.template";

    /// <inheritdoc />
    

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      var className = $"{instructions.SubjectName}V1Model";

      var template = GetTemplateAsStringBuilder(TemplateName);

      //Child templates
      template.Replace("{{Constructors}}", FillConstructors(instructions.Elections, className));

      SetAbsoluteContainingNamespace(template, instructions.ArchitectureStrategy, out var containingNamespace);

      SetUsingDirectives(
        template,
        instructions.ArchitectureStrategy,
        instructions.UsingDirectives,
        ResolutionMethod.Dynamic);

      //Full template replacements
      template.Replace("{{EntityName}}", instructions.EntityName);
      template.Replace("{{InterfaceName}}", instructions.InterfaceName);
      template.Replace("{{Interface}}", 
        instructions.Elections.HasFlag(GenerationElections.Interface) ? 
        FormatInterface(instructions.InterfaceName) : string.Empty);

      //Constructors
      template.Replace("{{ConstructorFromInterface}}", FormatConstructorBody(instructions.Properties, "target"));
      template.Replace("{{ConstructorFromEntity}}", FormatConstructorBody(instructions.Properties, "entity"));
      template.Replace("{{Properties}}", FormatProperties(instructions.Properties));
      
      return GetFormattedCSharpResult($"{className}.cs", template, containingNamespace);
    }

    private string FillConstructors(GenerationElections elections, string className)
    {
      var arr = new[]
      {
        //GenerationElections.GenerateInterface, //TODO: Not supporting this anymore
        GenerationElections.Entity,
      };

      var lst = new List<string>(arr.Length);

      foreach (var e in arr)
      {
        if (elections.HasFlag(e))
        {
          switch (e)
          {
            case GenerationElections.Entity:
              lst.Add(ConstructorTemplate(
                "{{SubjectName}}Entity",
                "entity",
                "{{ConstructorFromEntity}}",
                className));

              break;
          }
        }
      }

      if (lst.Count == 0) return string.Empty;

      //Only add in the default constructor, if and only if there are other constructors
      lst.Insert(0, ConstructorTemplate(
        string.Empty,
        string.Empty,
        string.Empty,
        className));

      return string.Join(Environment.NewLine + Environment.NewLine, lst);
    }
  }
}
