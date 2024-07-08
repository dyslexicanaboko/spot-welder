using SpotWelder.Lib.Models;
using System.Linq;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class ApiControllerGenerator
    : GeneratorBase
  {
    public ApiControllerGenerator(ClassInstructions instructions)
      : base(instructions, "ApiController.cs.template")
    {
      instructions.ClassName = instructions.SubjectName;
    }

    public override GeneratedResult FillTemplate()
    {
      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      template.Replace("{{Namespace}}", Instructions.Namespace);
      template.Replace("{{ApiRoute}}", Instructions.ApiRoute);
      template.Replace("{{SubjectName}}", Instructions.SubjectName);
      template.Replace("{{ClassName}}", Instructions.ClassName);
      template.Replace("{{ModelName}}", Instructions.EntityName);
      template.Replace("{{EntityName}}", Instructions.EntityName);
      template.Replace("{{InterfaceName}}", Instructions.InterfaceName);
      template.Replace("{{Namespaces}}", FormatNamespaces(Instructions.Namespaces));
      
      GetAsynchronicityFormatStrategy(Instructions.IsAsynchronous).ReplaceTags(template);

      var pk = Instructions.Properties.SingleOrDefault(x => x.IsPrimaryKey);

      if (pk != null)
      {
        template.Replace("{{PrimaryKeyProperty}}", pk.Property); //TaskId
        template.Replace("{{PrimaryKeyType}}", pk.SystemTypeAlias); //int
      }

      var t = template.ToString();

      t = RemoveExcessBlankSpace(t);
      
      return new GeneratedResult
      {
        Filename = $"{Instructions.ClassName}Controller.cs",
        Contents = t
      };
    }
  }
}
