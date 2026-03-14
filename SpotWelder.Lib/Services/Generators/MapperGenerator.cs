using SpotWelder.Lib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class MapperGenerator
    : GeneratorBase
  {
    //TODO: Remove the mappings
    private static readonly Dictionary<GenerationElections, TemplateInfo> ChildTemplates = new()
    {
      { GenerationElections.MapEntityToModel, new TemplateInfo("MapEntityToModel.cs.template", ["Entities", "Models"]) },
      { GenerationElections.MapModelToEntity, new TemplateInfo("MapModelToEntity.cs.template", ["Entities", "Models"]) },
      { GenerationElections.MapCreateModelToEntity, new TemplateInfo("MapCreateModelToEntity.cs.template", ["Entities", "Models.Client"]) },
      { GenerationElections.MapPatchModelToEntity, new TemplateInfo("MapPatchModelToEntity.cs.template", ["Entities", "Models.Client"]) },
      { GenerationElections.MapEntityToCreatedModel, new TemplateInfo("MapEntityToCreatedModel.cs.template", ["Entities", "Models.Client"]) },
      { GenerationElections.MapRecordToEntity, new TemplateInfo("MapRecordToEntity.cs.template", ["Entities", "Records"]) },
      { GenerationElections.MapEntityToRecord, new TemplateInfo("MapEntityToRecord.cs.template", ["Entities", "Records"]) }
    };

    /// <summary>
    ///   In this particular case the mapper accounts for these elections in one shot:
    ///   GenerationElections.CloneModelToEntity
    ///   GenerationElections.CloneEntityToModel
    ///   GenerationElections.CloneInterfaceToEntity
    ///   GenerationElections.CloneInterfaceToModel
    /// </summary>
    public override GenerationElections Election => GenerationElections.Mapper;

    protected override string TemplateName => "Mapper.cs.template";

    

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      var template = GetTemplateAsStringBuilder(TemplateName);

      SetAbsoluteContainingNamespace(template, instructions.ArchitectureStrategy, out var containingNamespace);

      SetUsingDirectivesDynamic(
        template,
        instructions.ArchitectureStrategy,
        instructions.UsingDirectives,
        instructions.Elections);

      template.Replace("{{Body}}", BuildBodyTemplate(instructions.Elections));
      template.Replace("{{SubjectName}}", instructions.SubjectName);
      template.Replace("{{EntityName}}", instructions.EntityName);
      template.Replace("{{InterfaceName}}", instructions.InterfaceName);
      template.Replace("{{ObjectInitializer}}", FormatObjectInitializerBody(instructions.Properties, "entity"));

      var result = GetFormattedCSharpResult($"{instructions.SubjectName}Mapper.cs", template, containingNamespace);

      result.CorrespondingInterface = GenerateInterface(
        instructions,
        result,
        "IMapper.cs.template");

      return result;
    }

    private string BuildBodyTemplate(GenerationElections elections)
    {
      var childElections = GetChildElections(elections, Election);

      var sb = new StringBuilder();
      
      foreach (var child in childElections)
      {
        sb
          .AppendLine(GetTemplate(ChildTemplates[child].TemplateName))
          .AppendLine();
      }
      
      return sb.ToString();
    }

    //FYI: 07/20/2024 This is a literal clone method, which I don't want to get rid of yet, but I won't be using right now.
    //There is a place for this, I am just not sure where yet. This is more of a DTO Maker thing,
    //but I could see needing this for scaffolding potentially too.
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
        p => $"      {to}.{p.Property} = {from}.{p.Property};",
        Environment.NewLine);

      return content;
    }

    private class TemplateInfo(string templateName, string[] namespaces)
    {
      public string TemplateName { get; set; } = templateName;

      public string[] Namespaces { get; set; } = namespaces;
    }
  }
}
