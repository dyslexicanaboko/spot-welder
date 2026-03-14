using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.CodeFactory;
using System.Collections.Generic;
using System;
using System.Text;
using System.Linq;

namespace SpotWelder.Lib.Services.Generators
{
  public class EntityGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.Entity;

    protected override string TemplateName => "Entity.cs.template";

    /// <inheritdoc />
    

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      var template = GetTemplateAsStringBuilder(TemplateName);

      //Child templates
      template.Replace("{{Interfaces}}", FillInterfaceImplementations(instructions.Elections));
      template.Replace("{{Constructors}}", FillConstructors(
        instructions.Elections,
        instructions.EntityName));
      template.Replace("{{InterfaceMethods}}", FillInterfaceMethods(instructions.Elections));

      SetAbsoluteContainingNamespace(template, instructions.ArchitectureStrategy, out var containingNamespace);

      //Depending on the elections, the using directives will change.
      SetUsingDirectivesDynamic(
        template,
        instructions.ArchitectureStrategy,
        instructions.UsingDirectives,
        instructions.Elections);

      //Full template replacements
      template.Replace("{{InterfaceName}}", instructions.InterfaceName);
      template.Replace("{{ClassAttributes}}", FormatClassAttributes(instructions.ClassAttributes));
      template.Replace("{{Properties}}", FormatProperties(instructions.Properties));

      //Constructors
      template.Replace("{{ConstructorFromInterface}}", FormatConstructorBody(instructions.Properties, "target"));
      template.Replace("{{ConstructorFromModel}}", FormatConstructorBody(instructions.Properties, "model"));
      template.Replace("{{ConstructorFromRecord}}", FormatConstructorBody(instructions.Properties, "record"));
      template.Replace("{{SubjectName}}", instructions.SubjectName);
      
      //IEquatable
      template.Replace("{{PropertiesEquals}}", FormatForEquals(instructions.Properties));
      template.Replace("{{PropertiesHashCode}}", FormatForHashCode(instructions.Properties));

      //IComparable
      //Property1 refers to "First property" which is arbitrary on purpose
      //The user is supposed to update the comparison logic based on their needs, but this serves as a starting point.
      template.Replace("{{Property1}}", instructions.Properties.First().Property);
      
      return GetFormattedCSharpResult($"{instructions.EntityName}.cs", template, containingNamespace);
    }

    private static string FillInterfaceImplementations(GenerationElections elections)
    {
      var arr = new[]
      {
        GenerationElections.Interface,
        GenerationElections.EntityIEquatable,
        GenerationElections.EntityIComparable
      };

      var lst = new List<string>(arr.Length);

      foreach (var e in arr)
      {
        if (elections.HasFlag(e))
        {
          switch (e)
          {
            case GenerationElections.Interface:
              lst.Add("{{InterfaceName}}");

              break;
            case GenerationElections.EntityIEquatable:
              lst.Add("IEquatable<{{SubjectName}}Entity>");

              break;
            case GenerationElections.EntityIComparable:
              lst.Add("IComparable");
           
              break;
          }
        }
      }

      if (lst.Count == 0) return string.Empty;

      return " : " + string.Join(", ", lst);
    }

    private string FillInterfaceMethods(GenerationElections elections)
    {
      var dict = new Dictionary<GenerationElections, string>
      {
        { GenerationElections.EntityIEquatable, "EntityIEquatable.cs.template" },
        { GenerationElections.EntityIComparable, "EntityIComparable.cs.template" }
      };

      var sb = new StringBuilder();

      foreach (var (election, template) in dict)
      {
        if(!elections.HasFlag(election)) continue;

        sb
          .AppendLine(GetTemplate(template))
          .AppendLine();
      }

      return sb.ToString();
    }

    private string FillConstructors(
      GenerationElections elections,
      string className)
    {
      var arr = new[]
      {
        GenerationElections.Interface,
        GenerationElections.Model,
        GenerationElections.CreateModel,
        GenerationElections.PatchModel,
        GenerationElections.Record
      };

      var lst = new List<string>(arr.Length);

      foreach (var e in arr)
      {
        if (elections.HasFlag(e))
        {
          switch (e)
          {
            case GenerationElections.Interface:
              lst.Add(ConstructorTemplate(
                "{{InterfaceName}}", 
                "target", 
                "{{ConstructorFromInterface}}",
                className));

              break;
            case GenerationElections.Record:
              lst.Add(ConstructorTemplate(
                "{{SubjectName}}Record",
                "record", 
                "{{ConstructorFromRecord}}",
                className));

              break;
            case GenerationElections.Model:
              lst.Add(ConstructorTemplate(
                "{{SubjectName}}V1Model",
                "model",
                "{{ConstructorFromModel}}",
                className));

              break;
            case GenerationElections.CreateModel:
              lst.Add(ConstructorTemplate(
                "{{SubjectName}}V1CreateModel",
                "model",
                "{{ConstructorFromModel}}", 
                className));

              break;
            case GenerationElections.PatchModel:
              lst.Add(ConstructorTemplate(
                "{{SubjectName}}V1PatchModel",
                "model",
                "{{ConstructorFromModel}}",
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
    
    private string FormatForEquals(List<ClassMemberStrings> properties)
    {
      var content = GetTextBlock(
        properties,
        p => $"                {p.Property} == other.{p.Property}",
        " && " + Environment.NewLine);

      return content;
    }

    private string FormatForHashCode(List<ClassMemberStrings> properties)
    {
      var content = GetTextBlock(
        properties,
        p => $"                {p.Property}.GetHashCode()",
        " + " + Environment.NewLine);

      return content;
    }
  }
}
