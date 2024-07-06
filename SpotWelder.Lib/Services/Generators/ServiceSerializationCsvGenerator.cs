using System.Text;
using SpotWelder.Lib.Models;

namespace SpotWelder.Lib.Services.Generators
{
    public class ServiceSerializationCsvGenerator
        : GeneratorBase
    {
        public ServiceSerializationCsvGenerator(ClassInstructions instructions)
            : base(instructions, "ServiceSerializationCsv.cs.template")
        {

        }

        public override GeneratedResult FillTemplate()
        {
            var strTemplate = GetTemplate(TemplateName);

            var template = new StringBuilder(strTemplate);

            template.Replace("{{Namespace}}", Instructions.Namespace);
            template.Replace("{{ClassName}}", Instructions.EntityName);

            var t = template.ToString();

            t = RemoveExcessBlankSpace(t);

            var r = GetResult();
            r.Filename = "SerializationService_Csv.cs";
            r.Contents = t;

            return r;
        }
    }
}
