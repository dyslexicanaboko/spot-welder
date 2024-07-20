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
    public override GenerationElections Election => GenerationElections.GenerateEntity;

    protected override string TemplateName => "Entity.cs.template";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      instructions.ClassName = instructions.EntityName;
      
      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      //Child templates
      template.Replace("{{Interfaces}}", FillInterfaceImplementations(instructions.Elections));
      template.Replace("{{Constructors}}", FillConstructors(instructions.Elections));
      template.Replace("{{InterfaceMethods}}", FillInterfaceMethods(instructions.Elections));

      //Full template replacements
      template.Replace("{{InterfaceName}}", instructions.InterfaceName);
      template.Replace("{{Namespace}}", instructions.Namespace);
      template.Replace("{{ClassName}}", instructions.ClassName);
      template.Replace("{{ModelName}}", instructions.ModelName);
      template.Replace("{{ClassAttributes}}", FormatClassAttributes(instructions.ClassAttributes));
      template.Replace("{{Namespaces}}", FormatNamespaces(instructions.Namespaces));
      template.Replace("{{Properties}}", FormatProperties(instructions.Properties));

      //Constructors
      template.Replace("{{ConstructorFromInterface}}", FormatConstructorBody(instructions.Properties, "target"));
      template.Replace("{{ConstructorFromModel}}", FormatConstructorBody(instructions.Properties, "model"));

      
      //IEquatable
      template.Replace("{{PropertiesEquals}}", FormatForEquals(instructions.Properties));
      template.Replace("{{PropertiesHashCode}}", FormatForHashCode(instructions.Properties));

      //IComparable
      template.Replace("{{Property1}}", instructions.Properties.First().Property);

      var t = template.ToString();

      t = RemoveExcessBlankSpace(t);
      
      return new($"{instructions.ClassName}.cs", t);
    }

    private static string FillInterfaceImplementations(GenerationElections elections)
    {
      var arr = new[]
      {
        GenerationElections.GenerateInterface,
        GenerationElections.GenerateEntityIEquatable,
        GenerationElections.GenerateEntityIComparable
      };

      var lst = new List<string>(arr.Length);

      foreach (var e in arr)
      {
        if (elections.HasFlag(e))
        {
          switch (e)
          {
            case GenerationElections.GenerateInterface:
              lst.Add("{{InterfaceName}}");

              break;
            case GenerationElections.GenerateEntityIEquatable:
              lst.Add("IEquatable<{{ClassName}}>");

              break;
            case GenerationElections.GenerateEntityIComparable:
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
        { GenerationElections.GenerateEntityIEquatable, "EntityIEquatable.cs.template" },
        { GenerationElections.GenerateEntityIComparable, "EntityIComparable.cs.template" }
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

    private string FillConstructors(GenerationElections elections)
    {
      var arr = new[]
      {
        GenerationElections.GenerateInterface,
        GenerationElections.GenerateModel,
        GenerationElections.GenerateCreateModel,
        GenerationElections.GeneratePatchModel
      };

      var lst = new List<string>(arr.Length);

      foreach (var e in arr)
      {
        if (elections.HasFlag(e))
        {
          switch (e)
          {
            case GenerationElections.GenerateInterface:
              lst.Add(ConstructorTemplate("{{InterfaceName}}", "target", "{{ConstructorFromInterface}}"));

              break;
            case GenerationElections.GenerateModel:
              lst.Add(ConstructorTemplate("{{ModelName}}", "model", "{{ConstructorFromModel}}"));

              break;
            case GenerationElections.GenerateCreateModel:
              lst.Add(ConstructorTemplate("{{ClassName}}V1CreateModel", "model", "{{ConstructorFromModel}}"));

              break;
            case GenerationElections.GeneratePatchModel:
              lst.Add(ConstructorTemplate("{{ClassName}}V1PatchModel", "model", "{{ConstructorFromModel}}"));

              break;
          }
        }
      }
      
      if (lst.Count == 0) return string.Empty;

      //Only add in the default constructor, if and only if there are other constructors
      lst.Insert(0, ConstructorTemplate(string.Empty, string.Empty, string.Empty));

      return string.Join(Environment.NewLine + Environment.NewLine, lst);
    }

    private string ConstructorTemplate(
      string parameterType, 
      string parameterName, 
      string constructorBody)
    {
      var sb = new StringBuilder(GetTemplate("EntityConstructor.cs.template"));

      //This is setting up a constructor template with tags for replacement
      sb
        .Replace("[ParameterType]", parameterType)
        .Replace("[ParameterName]", parameterName)
        .Replace("[ConstructorBody]", constructorBody);

      return sb.ToString();
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
