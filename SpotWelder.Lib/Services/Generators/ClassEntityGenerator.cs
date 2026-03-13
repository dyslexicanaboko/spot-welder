using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.CodeFactory;
using System.Collections.Generic;
using System;
using System.Text;
using System.Linq;

namespace SpotWelder.Lib.Services.Generators
{
  public class ClassEntityGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.Entity;

    protected override string TemplateName => "Entity.cs.template";

    /// <inheritdoc />
    protected override string ContainingNamespace => "Entities";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      instructions.ClassName = instructions.EntityName;
      
      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      //Child templates
      template.Replace("{{Interfaces}}", FillInterfaceImplementations(instructions.Elections));
      template.Replace("{{Constructors}}", FillConstructors(instructions.UsingDirectives, instructions.Elections));
      template.Replace("{{InterfaceMethods}}", FillInterfaceMethods(instructions.Elections));

      //Full template replacements
      SetContainingNamespace(template);
      template.Replace("{{InterfaceName}}", instructions.InterfaceName);
      template.Replace("{{Namespaces}}", FormatUsingDirectives(instructions.UsingDirectives));
      template.Replace("{{Namespace}}", instructions.RootContainingNamespace);
      template.Replace("{{ClassName}}", instructions.ClassName);
      template.Replace("{{ModelName}}", instructions.ModelName);
      template.Replace("{{RecordName}}", instructions.RecordName);
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
      template.Replace("{{Property1}}", instructions.Properties.First().Property);
      
      return GetFormattedCSharpResult($"{instructions.ClassName}.cs", template);
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
              lst.Add("IEquatable<{{ClassName}}>");

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

    private string FillConstructors(HashSet<string> usings, GenerationElections elections)
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
              lst.Add(ConstructorTemplate("{{InterfaceName}}", "target", "{{ConstructorFromInterface}}"));

              break;
            case GenerationElections.Record:
              usings.Add("{{Namespace}}.Records");
              lst.Add(ConstructorTemplate("{{RecordName}}", "record", "{{ConstructorFromRecord}}"));

              break;
            case GenerationElections.Model:
              usings.Add("{{Namespace}}.Models");
              lst.Add(ConstructorTemplate("{{ModelName}}", "model", "{{ConstructorFromModel}}"));

              break;
            case GenerationElections.CreateModel:
              usings.Add("{{Namespace}}.Models.Client");
              lst.Add(ConstructorTemplate("{{SubjectName}}V1CreateModel", "model", "{{ConstructorFromModel}}"));

              break;
            case GenerationElections.PatchModel:
              usings.Add("{{Namespace}}.Models.Client");
              lst.Add(ConstructorTemplate("{{SubjectName}}V1PatchModel", "model", "{{ConstructorFromModel}}"));

              break;
          }
        }
      }
      
      if (lst.Count == 0) return string.Empty;

      //Only add in the default constructor, if and only if there are other constructors
      lst.Insert(0, ConstructorTemplate(string.Empty, string.Empty, string.Empty));

      return string.Join(Environment.NewLine + Environment.NewLine, lst);
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
