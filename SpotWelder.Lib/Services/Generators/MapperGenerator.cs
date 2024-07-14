using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.CodeFactory;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class MapperGenerator
    : GeneratorBase
  {
    private readonly GenerationElections _services;

    public MapperGenerator(ClassInstructions instructions, GenerationElections services)
      : base(instructions, "Mapper.cs.template")
    {
      _services = services;
      
      instructions.ClassName = instructions.SubjectName;
    }

    public override GeneratedResult FillTemplate()
    {
      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      template.Replace("{{Namespace}}", Instructions.Namespace);
      template.Replace("{{ClassName}}", Instructions.ClassName);
      template.Replace("{{EntityName}}", Instructions.EntityName);
      template.Replace("{{ModelName}}", Instructions.ModelName);
      template.Replace("{{InterfaceName}}", Instructions.InterfaceName);
      template.Replace("{{Namespaces}}", FormatNamespaces(Instructions.Namespaces));

      //TODO: Keeping this stuff here for now - I might have to get rid of it.
      //Cloning method properties
      template.Replace(
        "{{PropertiesModelToEntity}}",
        FormatCloneBody(
          GenerationElections.CloneModelToEntity,
          Instructions.Properties,
          "model",
          "entity"));

      template.Replace(
        "{{PropertiesEntityToModel}}",
        FormatCloneBody(
          GenerationElections.CloneEntityToModel,
          Instructions.Properties,
          "entity",
          "model"));

      template.Replace(
        "{{PropertiesInterfaceToEntity}}",
        FormatCloneBody(
          GenerationElections.CloneInterfaceToEntity,
          Instructions.Properties,
          "target",
          "entity"));

      template.Replace(
        "{{PropertiesInterfaceToModel}}",
        FormatCloneBody(
          GenerationElections.CloneInterfaceToModel,
          Instructions.Properties,
          "target",
          "model"));

      var t = template.ToString();

      t = RemoveExcessBlankSpace(t);
      
      return new GeneratedResult
      {
        Filename = $"{Instructions.ClassName}Mapper.cs",
        Contents = t
      };
    }

    private string FormatCloneBody(
      GenerationElections flag,
      IList<ClassMemberStrings> properties,
      string from,
      string to)
    {
      if (!_services.HasFlag(flag))
      {
        var exception = GetNotImplementedException(
          $"Cloning option \"{flag}\" was excluded from generation. Delete this method.");

        return exception;
      }

      var content = GetTextBlock(
        properties,
        p => $"			{to}.{p.Property} = {from}.{p.Property};",
        Environment.NewLine);

      return content;
    }
  }
}
