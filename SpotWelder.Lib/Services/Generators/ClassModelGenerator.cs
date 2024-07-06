﻿using System.Text;
using SpotWelder.Lib.Models;

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
            template.Replace("{{ClassName}}", Instructions.EntityName);
            template.Replace("{{Interface}}", FormatInterface(Instructions.InterfaceName));
            template.Replace("{{Namespaces}}", FormatNamespaces(Instructions.Namespaces));

            var t = template.ToString();

            t = RemoveExcessBlankSpace(t);

            t = t.Replace("{{Properties}}", FormatProperties(Instructions.Properties));

            var r = GetResult();
            r.Contents = t;

            return r;
        }
    }
}
