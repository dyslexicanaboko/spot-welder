using SpotWelder.Lib.Models;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class ClassRecordGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.GenerateRecord;

    protected override string TemplateName => "Record.cs.template";

    /// <inheritdoc />
    protected override string ContainingNamespace => "Records";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      instructions.ClassName = instructions.RecordName;
      
      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      //Full template replacements
      SetContainingNamespace(template);
      template.Replace("{{Namespace}}", instructions.Namespace);
      template.Replace("{{ClassName}}", instructions.ClassName);
      template.Replace("{{Namespaces}}", FormatNamespaces(instructions.Namespaces));
      template.Replace("{{Properties}}", FormatPropertiesForRecord(instructions.Properties));
      
      return GetFormattedCSharpResult($"{instructions.ClassName}.cs", template);
    }
  }
}
