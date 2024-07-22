using SpotWelder.Lib.Models;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class ServiceSerializationCsvGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.SerializeCsv;

    protected override string TemplateName => "ServiceSerializationCsv.cs.template";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      template.Replace("{{Namespace}}", instructions.Namespace);
      template.Replace("{{ClassName}}", instructions.EntityName);

      return GetFormattedCSharpResult("SerializationService_Csv.cs", template);
    }
  }
}
