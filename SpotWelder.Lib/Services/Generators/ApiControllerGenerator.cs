using SpotWelder.Lib.Models;
using System.Linq;

namespace SpotWelder.Lib.Services.Generators
{
  public class ApiControllerGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.ApiController;

    protected override string TemplateName => "ApiController.cs.template";

    /// <inheritdoc />
    protected override string ContainingNamespace => "Controllers";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      instructions.ClassName = instructions.SubjectName;

      var templateName = TemplateName;

      /* When a query is provided, it's very likely it cannot handle CUD.
       * Therefore, this readonly template will be used. */
      if (instructions.SourceSqlType == SourceSqlType.Query)
        templateName = "ApiControllerReadsOnly.cs.template";

      var template = GetTemplateAsStringBuilder(templateName);

      //The same namespaces are always needed which is why it's static
      SetUsingDirectives(
        template,
        instructions.ArchitectureStrategy,
        instructions.UsingDirectives,
        ResolutionMethod.Static,
        templateName);

      SetContainingNamespace(template);
      template.Replace("{{RootContainingNamespace}}", instructions.RootContainingNamespace);
      template.Replace("{{ApiRoute}}", instructions.ApiRoute);
      template.Replace("{{SubjectName}}", instructions.SubjectName);
      template.Replace("{{ClassName}}", instructions.ClassName);
      template.Replace("{{ModelName}}", instructions.ModelName);
      template.Replace("{{EntityName}}", instructions.EntityName);
      template.Replace("{{InterfaceName}}", instructions.InterfaceName);
      template.Replace("{{UsingDirectives}}", FormatUsingDirectives(instructions.UsingDirectives));

      instructions.AsynchronicityFormatStrategy.ReplaceTags(template);

      var pk = instructions.Properties.SingleOrDefault(x => x.IsPrimaryKey);

      if (pk != null)
      {
        template.Replace("{{PrimaryKeyProperty}}", pk.Property); //TaskId
        template.Replace("{{PrimaryKeyType}}", pk.SystemTypeAlias); //int
      }

      return GetFormattedCSharpResult($"{instructions.ClassName}V1Controller.cs", template);
    }
  }
}
