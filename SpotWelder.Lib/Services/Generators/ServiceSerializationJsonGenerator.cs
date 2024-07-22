using SpotWelder.Lib.Models;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class ServiceSerializationJsonGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.SerializeJson;

    protected override string TemplateName => "ServiceSerializationJson.cs.template";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      template.Replace("{{Namespace}}", instructions.Namespace);
      template.Replace("{{ClassName}}", instructions.EntityName);
      
      return GetFormattedCSharpResult("SerializationService_Json.cs", template);
    }
  }
}
