using SpotWelder.Lib.Models;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class ClassEntityGenerator
    : GeneratorBase
  {
    public ClassEntityGenerator(ClassInstructions instructions)
      : base(instructions, "Entity.cs.template")
    {
    }

    public override GeneratedResult FillTemplate()
    {
      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      template.Replace("{{Namespace}}", Instructions.Namespace);
      template.Replace("{{ClassName}}", Instructions.ClassName);
      template.Replace("{{ModelName}}", Instructions.ModelName);
      template.Replace("{{InterfaceName}}", Instructions.InterfaceName);
      template.Replace("{{Partial}}", Instructions.IsPartial ? "partial " : string.Empty);
      template.Replace("{{Interface}}", FormatInterface(Instructions.InterfaceName));
      template.Replace("{{ClassAttributes}}", FormatClassAttributes(Instructions.ClassAttributes));
      template.Replace("{{Namespaces}}", FormatNamespaces(Instructions.Namespaces));

      //Constructors
      template.Replace("{{ConstructorFromInterface}}", FormatConstructorBody(Instructions.Properties, "target"));
      template.Replace("{{ConstructorFromModel}}", FormatConstructorBody(Instructions.Properties, "model"));

      var t = template.ToString();

      t = RemoveExcessBlankSpace(t);

      //t = RemoveBlankLines(t);

      t = t.Replace("{{Properties}}", FormatProperties(Instructions.Properties));

      var r = GetResult();
      r.Contents = t;

      return r;
    }
  }
}
