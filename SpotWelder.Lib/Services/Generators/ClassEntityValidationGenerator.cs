using SpotWelder.Lib.Models;

namespace SpotWelder.Lib.Services.Generators
{
  public class ClassEntityValidationGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.Validation;

    protected override string TemplateName => "EntityValidation.cs.template";

    /// <inheritdoc />
    

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      var template = GetTemplateAsStringBuilder(TemplateName);

      SetAbsoluteContainingNamespace(template, instructions.ArchitectureStrategy, out var containingNamespace);

      SetUsingDirectives(
        template,
        instructions.ArchitectureStrategy,
        instructions.UsingDirectives,
        ResolutionMethod.Static);

      template.Replace("{{SubjectName}}", instructions.SubjectName); //Subject is the prefix only

      //Validation of all properties by default
      template.Replace("{{Validation}}", FormatPropertiesForValidation(instructions.Properties));

      return GetFormattedCSharpResult($"{instructions.SubjectName}Validation.cs", template, containingNamespace);
    }
  }
}
