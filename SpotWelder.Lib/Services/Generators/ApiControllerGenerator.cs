using SpotWelder.Lib.Models;
using System.Linq;

namespace SpotWelder.Lib.Services.Generators
{
  public class ApiControllerGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.ApiController;

    protected override string TemplateName => "ApiController.cs.template";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      var templateName = TemplateName;

      /* When a query is provided, it's very likely it cannot handle CUD.
       * Therefore, this readonly template will be used. */
      if (instructions.SourceSqlType == SourceSqlType.Query)
        templateName = "ApiControllerReadsOnly.cs.template";

      var template = GetTemplateAsStringBuilder(templateName);

      SetAbsoluteContainingNamespace(template, instructions.ArchitectureStrategy, out var containingNamespace);

      //The same namespaces are always needed which is why it's static
      SetUsingDirectivesStatic(
        template,
        instructions.ArchitectureStrategy,
        instructions.UsingDirectives,
        templateName);

      template.Replace("{{ApiRoute}}", instructions.ApiRoute);
      template.Replace("{{SubjectName}}", instructions.SubjectName);
      template.Replace("{{EntityName}}", instructions.EntityName);
      template.Replace("{{InterfaceName}}", instructions.InterfaceName);

      instructions.AsynchronicityFormatStrategy.ReplaceTags(template);

      var pk = instructions.Properties.SingleOrDefault(x => x.IsPrimaryKey);

      if (pk != null)
      {
        template.Replace("{{PrimaryKeyProperty}}", pk.Property); //TaskId
        template.Replace("{{PrimaryKeyType}}", pk.SystemTypeAlias); //int
      }

      return GetFormattedCSharpResult($"{instructions.SubjectName}V1Controller.cs", template, containingNamespace);
    }
  }
}
