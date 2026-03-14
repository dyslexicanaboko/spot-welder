using SpotWelder.Lib.Models;

namespace SpotWelder.Lib.Services.Generators
{
  public class ServiceSerializationCsvGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.SerializeCsv;

    protected override string TemplateName => "ServiceSerializationCsv.cs.template";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      var template = GetTemplateAsStringBuilder(TemplateName);

      SetAbsoluteContainingNamespace(template, instructions.ArchitectureStrategy, out var containingNamespace);

      SetUsingDirectivesStatic(
        template,
        instructions.ArchitectureStrategy,
        instructions.UsingDirectives);

      template.Replace("{{EntityName}}", instructions.EntityName);

      return GetFormattedCSharpResult("SerializationService_Csv.cs", template, containingNamespace);
    }
  }
}
