using SpotWelder.Lib.Models;

namespace SpotWelder.Lib.Services.Generators
{
  public class ModelCreateGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.CreateModel;

    protected override string TemplateName => "ModelCreate.cs.template";

    /// <inheritdoc />
    

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      var template = GetTemplateAsStringBuilder(TemplateName);

      SetAbsoluteContainingNamespace(template, instructions.ArchitectureStrategy, out var containingNamespace);

      template.Replace("{{SubjectName}}", instructions.SubjectName);
      template.Replace("{{InterfaceName}}", instructions.InterfaceName);
      template.Replace("{{Interface}}",
        instructions.Elections.HasFlag(GenerationElections.Interface) ?
        FormatInterface(instructions.InterfaceName) : string.Empty);

      //Constructors
      template.Replace("{{ConstructorFromInterface}}", FormatConstructorBody(instructions.Properties, "target"));
      template.Replace("{{Properties}}", FormatProperties(instructions.Properties));

      return GetFormattedCSharpResult($"{instructions.SubjectName}V1CreateModel.cs", template, containingNamespace);
    }
  }
}
