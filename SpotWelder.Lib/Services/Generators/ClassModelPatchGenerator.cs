using SpotWelder.Lib.Models;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class ClassModelPatchGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.GeneratePatchModel;

    protected override string TemplateName => "ModelPatch.cs.template";

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

      var t = template.ToString();

      t = RemoveExcessBlankSpace(t);

      t = t.Replace("{{Properties}}", FormatProperties(instructions.Properties));

      return new($"{instructions.ClassName}V1PatchModel.cs", t);
    }
  }
}
