using System;
using System.Collections.Generic;
using System.Text;
using SimpleClassCreator.Lib.Models;
using SimpleClassCreator.Lib.Services.CodeFactory;

namespace SimpleClassCreator.Lib.Services.Generators
{
  public class LanguageTypeScriptGenerator
    : GeneratorBase
  {
    public LanguageTypeScriptGenerator(ClassInstructions instructions)
      : base(instructions, "Type.ts")
    {
    }

    public override GeneratedResult FillTemplate()
    {
      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      template.Replace("{{ClassName}}", Instructions.ClassEntityName);

      var t = template.ToString();

      t = RemoveExcessBlankSpace(t);

      t = t.Replace("{{Properties}}", FormatProperties(Instructions.Properties));

      var r = GetResult("ts");
      r.Contents = t;

      return r;
    }

    protected override string FormatProperties(IList<ClassMemberStrings> properties)
    {
      //FYI: The `Parameter` property is being used because it's camelCase
      var content = GetTextBlock(properties,
        (p) => $"    {p.Parameter}{(p.IsDbNullable ? "?" : string.Empty)}: {p.TypeScriptType};",
        separator: Environment.NewLine);

      return content;
    }
  }
}
