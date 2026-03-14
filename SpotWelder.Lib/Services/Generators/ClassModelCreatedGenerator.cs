using SpotWelder.Lib.Models;

namespace SpotWelder.Lib.Services.Generators
{
  public class ClassModelCreatedGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.CreatedModel;

    protected override string TemplateName => "ModelCreated.cs.template";

    /// <inheritdoc />
    protected override string ContainingNamespace => "Models.Client";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      var template = GetTemplateAsStringBuilder(TemplateName);
      
      SetAbsoluteContainingNamespace(template, instructions.ArchitectureStrategy);

      SetUsingDirectives(
        template,
        instructions.ArchitectureStrategy,
        instructions.UsingDirectives,
        ResolutionMethod.Static);

      template.Replace("{{SubjectName}}", instructions.SubjectName);
      template.Replace("{{EntityName}}", instructions.EntityName);

      //Constructors
      template.Replace("{{ConstructorFromEntity}}", FormatConstructorBody(instructions.Properties, "target"));
      template.Replace("{{Properties}}", FormatProperties(instructions.Properties));

      return GetFormattedCSharpResult($"{instructions.SubjectName}V1CreatedModel.cs", template);
    }
  }
}
