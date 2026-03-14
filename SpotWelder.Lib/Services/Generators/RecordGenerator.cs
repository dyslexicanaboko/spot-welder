using SpotWelder.Lib.Models;

namespace SpotWelder.Lib.Services.Generators
{
  public class RecordGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.Record;

    protected override string TemplateName => "Record.cs.template";

    /// <inheritdoc />
    

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      var template = GetTemplateAsStringBuilder(TemplateName);

      SetAbsoluteContainingNamespace(template, instructions.ArchitectureStrategy, out var containingNamespace);
      
      //Full template replacements
      template.Replace("{{SubjectName}}", instructions.SubjectName);
      template.Replace("{{Properties}}", FormatPropertiesForRecord(instructions.Properties));
      
      return GetFormattedCSharpResult($"{instructions.SubjectName}Record.cs", template, containingNamespace);
    }
  }
}
