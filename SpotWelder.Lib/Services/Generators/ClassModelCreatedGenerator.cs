using SpotWelder.Lib.Models;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
	public class ClassModelCreatedGenerator
		: GeneratorBase
	{
		public override GenerationElections Election => GenerationElections.CreatedModel;

		protected override string TemplateName => "ModelCreated.cs.template";

    /// <inheritdoc />
    protected override string ContainingNamespace => "Models.Client";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
		{
			instructions.ClassName = instructions.SubjectName;

			var strTemplate = GetTemplate(TemplateName);

			var template = new StringBuilder(strTemplate);

      SetContainingNamespace(template);
			template.Replace("{{Namespace}}", instructions.Namespace);
			template.Replace("{{ClassName}}", instructions.ClassName); //Subject is the prefix
			template.Replace("{{EntityName}}", instructions.EntityName); //Subject is the prefix
      template.Replace("{{Namespaces}}", FormatNamespaces(instructions.Namespaces));

			//Constructors
			template.Replace("{{ConstructorFromEntity}}", FormatConstructorBody(instructions.Properties, "target"));
			template.Replace("{{Properties}}", FormatProperties(instructions.Properties));

			return GetFormattedCSharpResult($"{instructions.ClassName}V1CreatedModel.cs", template);
		}
	}
}
