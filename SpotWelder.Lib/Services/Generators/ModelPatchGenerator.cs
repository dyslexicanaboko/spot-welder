using SpotWelder.Lib.Models;

namespace SpotWelder.Lib.Services.Generators
{
  public class ModelPatchGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.PatchModel;

    protected override string TemplateName => "ModelPatch.cs.template";

    /// <inheritdoc />
    

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      var template = GetTemplateAsStringBuilder(TemplateName);

      SetAbsoluteContainingNamespace(template, instructions.ArchitectureStrategy, out var containingNamespace);

      SetUsingDirectivesStatic(
        template,
        instructions.ArchitectureStrategy,
        instructions.UsingDirectives);

      template.Replace("{{SubjectName}}", instructions.SubjectName);
      template.Replace("{{EntityName}}", instructions.EntityName);
      template.Replace("{{InterfaceName}}", instructions.InterfaceName);
      template.Replace("{{Interface}}",
        instructions.Elections.HasFlag(GenerationElections.Interface) ?
        FormatInterface(instructions.InterfaceName) : string.Empty);

      //Constructors
      template.Replace("{{ConstructorFromEntity}}", FormatConstructorBody(instructions.Properties, "target"));
      template.Replace("{{Properties}}", FormatProperties(instructions.Properties));

      return GetFormattedCSharpResult($"{instructions.SubjectName}V1PatchModel.cs", template, containingNamespace);
    }
  }
}
