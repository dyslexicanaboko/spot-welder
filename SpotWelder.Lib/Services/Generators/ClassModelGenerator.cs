using SpotWelder.Lib.Models;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class ClassModelGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.GenerateModel;

    protected override string TemplateName => "Model.cs.template";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      instructions.ClassName = instructions.ModelName;

      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      template.Replace("{{Namespace}}", instructions.Namespace);
      template.Replace("{{ClassName}}", instructions.ClassName);
      template.Replace("{{EntityName}}", instructions.EntityName);
      template.Replace("{{InterfaceName}}", instructions.InterfaceName);
      template.Replace("{{Interface}}", FormatInterface(instructions.InterfaceName));
      template.Replace("{{Namespaces}}", FormatNamespaces(instructions.Namespaces));

      //Constructors
      template.Replace("{{ConstructorFromInterface}}", FormatConstructorBody(instructions.Properties, "target"));
      template.Replace("{{ConstructorFromEntity}}", FormatConstructorBody(instructions.Properties, "entity"));

      var t = template.ToString();

      t = RemoveExcessBlankSpace(t);

      t = t.Replace("{{Properties}}", FormatProperties(instructions.Properties));

      var r = GetResult(instructions.ClassName);
      r.Contents = t;

      return r;
    }
  }
}
