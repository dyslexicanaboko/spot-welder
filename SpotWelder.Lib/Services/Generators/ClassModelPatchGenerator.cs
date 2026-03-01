using SpotWelder.Lib.Models;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
	public class ClassModelPatchGenerator
		: GeneratorBase
	{
		public override GenerationElections Election => GenerationElections.GeneratePatchModel;

		protected override string TemplateName => "ModelPatch.cs.template";

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
			template.Replace("{{InterfaceName}}", instructions.InterfaceName);
      template.Replace("{{Interface}}",
        instructions.Elections.HasFlag(GenerationElections.GenerateInterface) ?
        FormatInterface(instructions.InterfaceName) : string.Empty);
      template.Replace("{{Namespaces}}", FormatNamespaces(instructions.Namespaces));

			//Constructors
			template.Replace("{{ConstructorFromInterface}}", FormatConstructorBody(instructions.Properties, "target"));
			template.Replace("{{Properties}}", FormatProperties(instructions.Properties));

			return GetFormattedCSharpResult($"{instructions.ClassName}V1PatchModel.cs", template);
		}
	}
}
