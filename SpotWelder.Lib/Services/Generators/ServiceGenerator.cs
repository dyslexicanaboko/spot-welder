using SpotWelder.Lib.Models;
using System.Linq;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class ServiceGenerator
    : GeneratorBase
  {
    public ServiceGenerator(ClassInstructions instructions)
      : base(instructions, "Service.cs.template")
    {
      instructions.ClassName = instructions.SubjectName;
    }

    public override GeneratedResult FillTemplate()
    {
      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      template.Replace("{{Namespace}}", Instructions.Namespace);
      template.Replace("{{ClassName}}", Instructions.ClassName);
      template.Replace("{{EntityName}}", Instructions.EntityName);
      template.Replace("{{Namespaces}}", FormatNamespaces(Instructions.Namespaces));
      
      GetAsynchronicityFormatStrategy(Instructions.IsAsynchronous).ReplaceTags(template);

      var pk = Instructions.Properties.SingleOrDefault(x => x.IsPrimaryKey);

      if (pk != null)
      {
        template.Replace("{{PrimaryKeyParameter}}", pk.Parameter); //taskId
        template.Replace("{{PrimaryKeyProperty}}", pk.Property); //TaskId
        template.Replace("{{PrimaryKeyType}}", pk.SystemTypeAlias); //int
      }

      var t = template.ToString();

      t = RemoveExcessBlankSpace(t);
      
      return new GeneratedResult
      {
        Filename = $"{Instructions.ClassName}Service.cs",
        Contents = t
      };
    }
  }
}
