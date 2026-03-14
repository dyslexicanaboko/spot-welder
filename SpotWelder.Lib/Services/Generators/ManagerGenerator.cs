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

    

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      var templateName = TemplateName;

      /* When a query is provided, it's very likely it cannot handle CUD.
       * Therefore, this readonly template will be used. */
      if (instructions.SourceSqlType == SourceSqlType.Query)
        templateName = "ManagerReadsOnly.cs.template";

      var template = GetTemplateAsStringBuilder(templateName);

      SetAbsoluteContainingNamespace(template, instructions.ArchitectureStrategy, out var containingNamespace);

      //Depending on the elections, the using directives will change.
      SetUsingDirectivesStatic(
        template,
        instructions.ArchitectureStrategy,
        instructions.UsingDirectives,
        templateName);

      template.Replace("{{SubjectName}}", instructions.SubjectName);
      template.Replace("{{EntityName}}", instructions.EntityName);

      instructions.AsynchronicityFormatStrategy.ReplaceTags(template);

      var pk = instructions.Properties.SingleOrDefault(x => x.IsPrimaryKey);

      if (pk != null)
      {
        template.Replace("{{PrimaryKeyParameter}}", pk.Parameter); //taskId
        template.Replace("{{PrimaryKeyProperty}}", pk.Property); //TaskId
        template.Replace("{{PrimaryKeyType}}", pk.SystemTypeAlias); //int
      }

      var result = GetFormattedCSharpResult($"{instructions.SubjectName}Manager.cs", template, containingNamespace);

      result.CorrespondingInterface = GenerateInterface(
        instructions,
        result,
        "IManager.cs.template");

      return result;
    }
  }
}
