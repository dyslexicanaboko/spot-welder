using SpotWelder.Lib.Models;
using System;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class MapperGenerator
    : GeneratorBase
  {
    /// <summary>
    /// In this particular case the mapper accounts for these elections in one shot:
    ///   GenerationElections.CloneModelToEntity
    ///   GenerationElections.CloneEntityToModel
    ///   GenerationElections.CloneInterfaceToEntity
    ///   GenerationElections.CloneInterfaceToModel
    /// </summary>
    public override GenerationElections Election => GenerationElections.GenerateMapper;
    
    protected override string TemplateName => "Mapper.cs.template";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      template.Replace("{{Namespace}}", instructions.Namespace);
      template.Replace("{{ClassName}}", instructions.ClassName);
      template.Replace("{{EntityName}}", instructions.EntityName);
      template.Replace("{{ModelName}}", instructions.ModelName);
      template.Replace("{{InterfaceName}}", instructions.InterfaceName);
      template.Replace("{{Namespaces}}", FormatNamespaces(instructions.Namespaces));

      //Cloning method properties
      template.Replace(
        "{{PropertiesModelToEntity}}",
        FormatCloneBody(
          GenerationElections.CloneModelToEntity,
          instructions,
          "model",
          "entity"));

      template.Replace(
        "{{PropertiesEntityToModel}}",
        FormatCloneBody(
          GenerationElections.CloneEntityToModel,
          instructions,
          "entity",
          "model"));

      template.Replace(
        "{{PropertiesInterfaceToEntity}}",
        FormatCloneBody(
          GenerationElections.CloneInterfaceToEntity,
          instructions,
          "target",
          "entity"));

      template.Replace(
        "{{PropertiesInterfaceToModel}}",
        FormatCloneBody(
          GenerationElections.CloneInterfaceToModel,
          instructions,
          "target",
          "model"));

      var t = template.ToString();

      t = RemoveExcessBlankSpace(t);

      return new($"{instructions.ClassName}Mapper.cs", t);
    }

    private string FormatCloneBody(
      GenerationElections flag,
      ClassInstructions instructions,
      string from,
      string to)
    {
      if (!instructions.Elections.HasFlag(flag))
      {
        var exception = GetNotImplementedException(
          $"Cloning option \"{flag}\" was excluded from generation. Delete this method.");

        return exception;
      }

      var content = GetTextBlock(
        instructions.Properties,
        p => $"			{to}.{p.Property} = {from}.{p.Property};",
        Environment.NewLine);

      return content;
    }
  }
}
