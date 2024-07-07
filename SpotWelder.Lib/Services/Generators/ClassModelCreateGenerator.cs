using SpotWelder.Lib.Models;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class ClassModelCreateGenerator
    : GeneratorBase
  {
    public ClassModelCreateGenerator(ClassInstructions instructions)
      : base(instructions, "ModelCreate.cs.template")
    {
      instructions.ClassName = instructions.SubjectName;
    }

    public override GeneratedResult FillTemplate()
    {
      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      template.Replace("{{Namespace}}", Instructions.Namespace);
      template.Replace("{{ClassName}}", Instructions.ClassName); //Subject is the prefix
      template.Replace("{{InterfaceName}}", Instructions.InterfaceName);
      template.Replace("{{Namespaces}}", FormatNamespaces(Instructions.Namespaces));

      //Constructors
      template.Replace("{{ConstructorFromInterface}}", FormatConstructorBody(Instructions.Properties, "target"));

      var t = template.ToString();

      t = RemoveExcessBlankSpace(t);

      t = t.Replace("{{Properties}}", FormatProperties(Instructions.Properties));

      return new GeneratedResult
      {
        Filename = $"{Instructions.ClassName}V1CreateModel.cs",
        Contents = t
      };
    }
  }
}
