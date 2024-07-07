﻿using System;
using System.Collections.Generic;
using System.Text;
using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.CodeFactory;

namespace SpotWelder.Lib.Services.Generators
{
    public class ClassInterfaceGenerator
        : GeneratorBase
    {
        public ClassInterfaceGenerator(ClassInstructions instructions)
            : base(instructions, "Interface.cs.template")
        {

        }

        public override GeneratedResult FillTemplate()
        {
            var strTemplate = GetTemplate(TemplateName);

            var template = new StringBuilder(strTemplate);

            template.Replace("{{Namespace}}", Instructions.Namespace);
            template.Replace("{{ClassName}}", Instructions.ClassName);
            template.Replace("{{Namespaces}}", FormatNamespaces(Instructions.Namespaces));

            var t = template.ToString();

            t = RemoveExcessBlankSpace(t);

            t = t.Replace("{{Properties}}", FormatProperties(Instructions.Properties));

            var r = GetResult();
            r.Contents = t;

            return r;
        }

        protected override string FormatProperties(IList<ClassMemberStrings> properties)
        {
            var content = GetTextBlock(properties, 
                (p) => $"        {p.SystemTypeAlias} {p.Property} {{ get; set; }}", 
                separator: Environment.NewLine + Environment.NewLine);

            return content;
        }
    }
}
