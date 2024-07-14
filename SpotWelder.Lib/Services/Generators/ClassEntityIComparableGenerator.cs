using SpotWelder.Lib.Models;
using System.Linq;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class ClassEntityIComparableGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.GenerateEntityIComparable;

    protected override string TemplateName => "EntityIComparable.cs.template";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      instructions.ClassName = instructions.EntityName;

      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      template.Replace("{{Namespace}}", instructions.Namespace);
      template.Replace("{{ClassName}}", instructions.EntityName);
      template.Replace("{{EntityName}}", instructions.EntityName);
      template.Replace("{{Namespaces}}", FormatNamespaces(instructions.Namespaces));

      var t = template.ToString();

      t = RemoveExcessBlankSpace(t);

      t = t.Replace("{{Property1}}", instructions.Properties.First().Property);

      return new(instructions.EntityName + "_IComparable.cs", t);
    }
  }
}
