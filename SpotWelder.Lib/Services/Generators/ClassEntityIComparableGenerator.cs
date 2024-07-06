using System.Linq;
using System.Text;
using SpotWelder.Lib.Models;

namespace SpotWelder.Lib.Services.Generators
{
    public class ClassEntityIComparableGenerator
        : GeneratorBase
    {
        public ClassEntityIComparableGenerator(ClassInstructions instructions)
            : base(instructions, "EntityIComparable.cs.template")
        {

        }

        public override GeneratedResult FillTemplate()
        {
            var strTemplate = GetTemplate(TemplateName);

            var template = new StringBuilder(strTemplate);

            template.Replace("{{Namespace}}", Instructions.Namespace);
            template.Replace("{{ClassName}}", Instructions.EntityName);
            template.Replace("{{Namespaces}}", FormatNamespaces(Instructions.Namespaces));

            var t = template.ToString();

            t = RemoveExcessBlankSpace(t);

            t = t.Replace("{{Property1}}", Instructions.Properties.First().Property);

            var r = GetResult();
            r.Filename = Instructions.EntityName + "_IComparable.cs";
            r.Contents = t;

            return r;
        }
    }
}
