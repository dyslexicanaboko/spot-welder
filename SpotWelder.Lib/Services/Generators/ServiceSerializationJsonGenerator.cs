using SpotWelder.Lib.Models;

namespace SpotWelder.Lib.Services.Generators
{
  public class ServiceSerializationJsonGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.SerializeJson;

    protected override string TemplateName => "ServiceSerializationJson.cs.template";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      var template = GetTemplateAsStringBuilder(TemplateName);

      SetAbsoluteContainingNamespace(template, instructions.ArchitectureStrategy);

      template.Replace("{{EntityName}}", instructions.EntityName);
      
      return GetFormattedCSharpResult("SerializationService_Json.cs", template);
    }
  }
}
