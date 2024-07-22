using SpotWelder.Lib.Models;
using System.Collections.Generic;
using System;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class ClassModelGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.GenerateModel;

    protected override string TemplateName => "Model.cs.template";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      instructions.ClassName = instructions.ModelName;

      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      //Child templates
      template.Replace("{{Constructors}}", FillConstructors(instructions.Elections));

      //Full template replacements
      template.Replace("{{Namespace}}", instructions.Namespace);
      template.Replace("{{ClassName}}", instructions.ClassName);
      template.Replace("{{EntityName}}", instructions.EntityName);
      template.Replace("{{InterfaceName}}", instructions.InterfaceName);
      template.Replace("{{Interface}}", 
        instructions.Elections.HasFlag(GenerationElections.GenerateInterface) ? 
        FormatInterface(instructions.InterfaceName) : string.Empty);
      template.Replace("{{Namespaces}}", FormatNamespaces(instructions.Namespaces));

      //Constructors
      template.Replace("{{ConstructorFromInterface}}", FormatConstructorBody(instructions.Properties, "target"));
      template.Replace("{{ConstructorFromEntity}}", FormatConstructorBody(instructions.Properties, "entity"));

      var t = template.ToString();

      t = RemoveExcessBlankSpace(t);

      t = t.Replace("{{Properties}}", FormatProperties(instructions.Properties));

      var r = GetResult(instructions.ClassName);
      r.Contents = t;

      return r;
    }

    private string FillConstructors(GenerationElections elections)
    {
      var arr = new[]
      {
        GenerationElections.GenerateInterface,
        GenerationElections.GenerateEntity,
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
            case GenerationElections.GenerateEntity:
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
