using SpotWelder.Lib.Models;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class ClassEntityGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.GenerateEntity;

    protected override string TemplateName => "Entity.cs.template";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      instructions.IsPartial = 
        instructions.Elections.HasFlag(GenerationElections.GenerateEntityIEquatable) ||
        instructions.Elections.HasFlag(GenerationElections.GenerateEntityIComparable);

      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      template.Replace("{{Namespace}}", instructions.Namespace);
      template.Replace("{{ClassName}}", instructions.ClassName);
      template.Replace("{{ModelName}}", instructions.ModelName);
      template.Replace("{{InterfaceName}}", instructions.InterfaceName);
      template.Replace("{{Partial}}", instructions.IsPartial ? "partial " : string.Empty);
      template.Replace("{{Interface}}", FormatInterface(instructions.InterfaceName));
      template.Replace("{{ClassAttributes}}", FormatClassAttributes(instructions.ClassAttributes));
      template.Replace("{{Namespaces}}", FormatNamespaces(instructions.Namespaces));

      //Constructors
      template.Replace("{{ConstructorFromInterface}}", FormatConstructorBody(instructions.Properties, "target"));
      template.Replace("{{ConstructorFromModel}}", FormatConstructorBody(instructions.Properties, "model"));

      var t = template.ToString();

      t = RemoveExcessBlankSpace(t);

      //t = RemoveBlankLines(t);

      t = t.Replace("{{Properties}}", FormatProperties(instructions.Properties));

      var r = GetResult(instructions.ClassName);
      r.Contents = t;

      return r;
    }
  }
}
