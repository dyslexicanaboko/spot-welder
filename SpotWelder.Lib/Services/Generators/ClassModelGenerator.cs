using SpotWelder.Lib.Models;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class ClassModelGenerator
    : GeneratorBase
  {
    public ClassModelGenerator(ClassInstructions instructions)
      : base(instructions, "Model.cs.template")
    {
    }

    public override GeneratedResult FillTemplate()
    {
      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      template.Replace("{{Namespace}}", Instructions.Namespace);
      template.Replace("{{ClassName}}", Instructions.ClassName);
      template.Replace("{{EntityName}}", Instructions.EntityName);
      template.Replace("{{InterfaceName}}", Instructions.InterfaceName);
      template.Replace("{{Interface}}", FormatInterface(Instructions.InterfaceName));
      template.Replace("{{Namespaces}}", FormatNamespaces(Instructions.Namespaces));

      //Constructors
      template.Replace("{{ConstructorFromInterface}}", FormatConstructorBody(Instructions.Properties, "target"));
      template.Replace("{{ConstructorFromEntity}}", FormatConstructorBody(Instructions.Properties, "entity"));

      var t = template.ToString();

      t = RemoveExcessBlankSpace(t);

      t = t.Replace("{{Properties}}", FormatProperties(Instructions.Properties));

      var r = GetResult();
      r.Contents = t;

      return r;
    }
  }
}
