using SpotWelder.Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class MapperGenerator
    : GeneratorBase
  {
    private static readonly Dictionary<GenerationElections, TemplateInfo> ChildTemplates = new()
    {
      { GenerationElections.MapEntityToModel, new TemplateInfo("MapEntityToModel.cs.template", ["Entities", "Models"]) },
      { GenerationElections.MapModelToEntity, new TemplateInfo("MapModelToEntity.cs.template", ["Entities", "Models"]) },

      //FYI: Commenting out these two mapping options for now as they are just confusing things.
      //I might eliminate these entirely later. They have not been useful in practice.
      //{ GenerationElections.MapInterfaceToEntity, "MapInterfaceToEntity.cs.template" },
      //{ GenerationElections.MapInterfaceToModel, "MapInterfaceToModel.cs.template" },
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
    public override GenerationElections Election => GenerationElections.GenerateMapper;

    protected override string TemplateName => "Mapper.cs.template";

    protected override string ContainingNamespace => "Mappers";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      instructions.ClassName = instructions.SubjectName;

      var template = new StringBuilder(GetTemplate(TemplateName));

      template.Replace("{{Body}}", BuildBodyTemplate(instructions.Namespaces, instructions.Elections));
      SetContainingNamespace(template);
      template.Replace("{{Namespaces}}", FormatNamespaces(instructions.Namespaces));
      template.Replace("{{Namespace}}", instructions.Namespace);
      template.Replace("{{ClassName}}", instructions.ClassName);
      template.Replace("{{EntityName}}", instructions.EntityName);
      template.Replace("{{RecordName}}", instructions.RecordName);
      template.Replace("{{ModelName}}", instructions.ModelName);
      template.Replace("{{InterfaceName}}", instructions.InterfaceName);
      template.Replace("{{ObjectInitializer}}", FormatObjectInitializerBody(instructions.Properties, "entity"));

      var result = GetFormattedCSharpResult($"{instructions.ClassName}Mapper.cs", template);

      result.CorrespondingInterface = GenerateInterface(
        instructions,
        result,
        "IMapper.cs.template");

      return result;
    }

    private string BuildBodyTemplate(IList<string> namespaces, GenerationElections elections)
    {
      var childElections = GetChildElections(elections, Election);

      var sb = new StringBuilder();
      var hs = new HashSet<string>();

      foreach (var child in childElections)
      {
        var templateInfo = ChildTemplates[child];

        var arr = templateInfo.Namespaces.Select(x => $"{{{{Namespace}}}}.{x}").ToArray();

        //HashSet will prevent duplicates.
        foreach (var ns in arr)
          hs.Add(ns);

        sb
          .AppendLine(GetTemplate(templateInfo.TemplateName))
          .AppendLine();
      }

      //Add to main list of namespaces
      foreach (var ns in hs)
        namespaces.Add(ns);
      
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
        p => $"			{to}.{p.Property} = {from}.{p.Property};",
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
