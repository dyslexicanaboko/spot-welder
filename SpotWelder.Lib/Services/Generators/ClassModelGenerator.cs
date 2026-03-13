using SpotWelder.Lib.Models;
using System.Collections.Generic;
using System;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class ClassModelGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.Model;

    protected override string TemplateName => "Model.cs.template";

    /// <inheritdoc />
    protected override string ContainingNamespace => "Models";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      instructions.ClassName = instructions.ModelName;

      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      //Child templates
      template.Replace("{{Constructors}}", FillConstructors(instructions.Elections));

      //Full template replacements
      SetContainingNamespace(template);
      template.Replace("{{RootContainingNamespace}}", instructions.RootContainingNamespace);
      template.Replace("{{ClassName}}", instructions.ClassName);
      template.Replace("{{EntityName}}", instructions.EntityName);
      template.Replace("{{InterfaceName}}", instructions.InterfaceName);
      template.Replace("{{Interface}}", 
        instructions.Elections.HasFlag(GenerationElections.Interface) ? 
        FormatInterface(instructions.InterfaceName) : string.Empty);
      template.Replace("{{UsingDirectives}}", FormatUsingDirectives(instructions.UsingDirectives));

      //Constructors
      template.Replace("{{ConstructorFromInterface}}", FormatConstructorBody(instructions.Properties, "target"));
      template.Replace("{{ConstructorFromEntity}}", FormatConstructorBody(instructions.Properties, "entity"));
      template.Replace("{{Properties}}", FormatProperties(instructions.Properties));
      
      return GetFormattedCSharpResult($"{instructions.ClassName}.cs", template);
    }

    private string FillConstructors(GenerationElections elections)
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
            //TODO: Not supporting this anymore
            case GenerationElections.Interface:
              lst.Add(ConstructorTemplate("{{InterfaceName}}", "target", "{{ConstructorFromInterface}}"));

              break;
            case GenerationElections.Entity:
              lst.Add(ConstructorTemplate("{{EntityName}}", "entity", "{{ConstructorFromEntity}}"));

              break;
          }
        }
      }

      if (lst.Count == 0) return string.Empty;

      //Only add in the default constructor, if and only if there are other constructors
      lst.Insert(0, ConstructorTemplate(string.Empty, string.Empty, string.Empty));

      return string.Join(Environment.NewLine + Environment.NewLine, lst);
    }
  }
}
