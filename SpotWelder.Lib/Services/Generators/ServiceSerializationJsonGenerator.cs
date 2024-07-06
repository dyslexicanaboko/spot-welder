using System.Text;
using SpotWelder.Lib.Models;

namespace SpotWelder.Lib.Services.Generators
{
    public class ServiceSerializationJsonGenerator
        : GeneratorBase
    {
        public ServiceSerializationJsonGenerator(ClassInstructions instructions)
            : base(instructions, "ServiceSerializationJson.cs.template")
        {

        }

        public override GeneratedResult FillTemplate()
        {
            var strTemplate = GetTemplate(TemplateName);

            var template = new StringBuilder(strTemplate);

            template.Replace("{{Namespace}}", Instructions.Namespace);
            template.Replace("{{ClassName}}", Instructions.ClassEntityName);

            var t = template.ToString();

            t = RemoveExcessBlankSpace(t);

            var r = GetResult();
            r.Filename = "SerializationService_Json.cs";
            r.Contents = t;

            return r;
        }
    }
}
