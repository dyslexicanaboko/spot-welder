using SpotWelder.Lib.Models;
using System.Linq;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class ApiControllerGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.ApiController;

    protected override string TemplateName => "ApiController.cs.template";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      instructions.ClassName = instructions.SubjectName;

      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      template.Replace("{{Namespace}}", instructions.Namespace);
      template.Replace("{{ApiRoute}}", instructions.ApiRoute);
      template.Replace("{{SubjectName}}", instructions.SubjectName);
      template.Replace("{{ClassName}}", instructions.ClassName);
      template.Replace("{{ModelName}}", instructions.EntityName);
      template.Replace("{{EntityName}}", instructions.EntityName);
      template.Replace("{{InterfaceName}}", instructions.InterfaceName);
      template.Replace("{{Namespaces}}", FormatNamespaces(instructions.Namespaces));

      GetAsynchronicityFormatStrategy(instructions.IsAsynchronous).ReplaceTags(template);

      var pk = instructions.Properties.SingleOrDefault(x => x.IsPrimaryKey);

      if (pk != null)
      {
        template.Replace("{{PrimaryKeyProperty}}", pk.Property); //TaskId
        template.Replace("{{PrimaryKeyType}}", pk.SystemTypeAlias); //int
      }

      var t = template.ToString();

      t = RemoveExcessBlankSpace(t);

      return new GeneratedResult($"{instructions.ClassName}Controller.cs", t);
    }
  }
}
