using SpotWelder.Lib.Models;
using System.Linq;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class ManagerGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.Manager;

    protected override string TemplateName => "Manager.cs.template";

    protected override string ContainingNamespace => "Business";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      instructions.ClassName = instructions.SubjectName;

      var templateName = TemplateName;

      /* When a query is provided, it's very likely it cannot handle CUD.
       * Therefore, this readonly template will be used. */
      if (instructions.SourceSqlType == SourceSqlType.Query)
        templateName = "ManagerReadsOnly.cs.template";

      var template = new StringBuilder(GetTemplate(templateName));

      SetContainingNamespace(template);
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

      var result = GetFormattedCSharpResult($"{instructions.ClassName}Manager.cs", template);

      result.CorrespondingInterface = GenerateInterface(
        instructions,
        result,
        "IManager.cs.template");

      return result;
    }
  }
}
