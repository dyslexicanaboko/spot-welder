using SpotWelder.Lib.Models;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class ClassEntityValidationGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.Validation;

    protected override string TemplateName => "EntityValidation.cs.template";

    /// <inheritdoc />
    protected override string ContainingNamespace => "Validation";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      SetContainingNamespace(template);
      template.Replace("{{RootContainingNamespace}}", instructions.RootContainingNamespace);
      template.Replace("{{SubjectName}}", instructions.SubjectName); //Subject is the prefix
      template.Replace("{{UsingDirectives}}", FormatUsingDirectives(instructions.UsingDirectives));

      //Validation of all properties by default
      template.Replace("{{Validation}}", FormatPropertiesForValidation(instructions.Properties));

      return GetFormattedCSharpResult($"{instructions.SubjectName}Validation.cs", template);
    }
  }
}
