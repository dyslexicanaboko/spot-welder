using SpotWelder.Lib.Models;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
	public class ClassModelCreateGenerator
		: GeneratorBase
	{
		public override GenerationElections Election => GenerationElections.GenerateCreateModel;

		protected override string TemplateName => "ModelCreate.cs.template";

		public override GeneratedResult FillTemplate(ClassInstructions instructions)
		{
			instructions.ClassName = instructions.SubjectName;

			var strTemplate = GetTemplate(TemplateName);

			var template = new StringBuilder(strTemplate);

			template.Replace("{{Namespace}}", instructions.Namespace);
			template.Replace("{{ClassName}}", instructions.ClassName); //Subject is the prefix
			template.Replace("{{InterfaceName}}", instructions.InterfaceName);
			template.Replace("{{Namespaces}}", FormatNamespaces(instructions.Namespaces));

			//Constructors
			template.Replace("{{ConstructorFromInterface}}", FormatConstructorBody(instructions.Properties, "target"));
			template.Replace("{{Properties}}", FormatProperties(instructions.Properties));

			return GetFormattedCSharpResult($"{instructions.ClassName}V1CreateModel.cs", template);
		}
	}
}
