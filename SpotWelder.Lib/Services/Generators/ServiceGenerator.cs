using SpotWelder.Lib.Models;
using System.Linq;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class ServiceGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.Service;

    protected override string TemplateName => "Service.cs.template";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      template.Replace("{{Namespace}}", instructions.Namespace);
      template.Replace("{{ClassName}}", instructions.ClassName);
      template.Replace("{{EntityName}}", instructions.EntityName);
      template.Replace("{{Namespaces}}", FormatNamespaces(instructions.Namespaces));

      GetAsynchronicityFormatStrategy(instructions.IsAsynchronous).ReplaceTags(template);

      var pk = instructions.Properties.SingleOrDefault(x => x.IsPrimaryKey);

      if (pk != null)
      {
        template.Replace("{{PrimaryKeyParameter}}", pk.Parameter); //taskId
        template.Replace("{{PrimaryKeyProperty}}", pk.Property); //TaskId
        template.Replace("{{PrimaryKeyType}}", pk.SystemTypeAlias); //int
      }
      
      return GetFormattedCSharpResult($"{instructions.ClassName}Service.cs", template);
    }
  }
}
