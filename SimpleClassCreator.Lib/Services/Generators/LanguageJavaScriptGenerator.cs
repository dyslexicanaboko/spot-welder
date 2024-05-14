using System;
using System.Collections.Generic;
using System.Text;
using SimpleClassCreator.Lib.Models;
using SimpleClassCreator.Lib.Services.CodeFactory;

namespace SimpleClassCreator.Lib.Services.Generators
{
  public class LanguageJavaScriptGenerator
    : GeneratorBase
  {
    public LanguageJavaScriptGenerator(ClassInstructions instructions)
      : base(instructions, "Prototype.js")
    {
    }

    public override GeneratedResult FillTemplate()
    {
      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      template.Replace("{{ClassName}}", Instructions.ClassEntityName);

      var t = template.ToString();

      t = RemoveExcessBlankSpace(t);

      //FYI: The `Parameter` property is being used because it's camelCase
      t = t.Replace("{{Parameters}}", FormatParameters(Instructions.Properties));
      t = t.Replace("{{Properties}}", FormatProperties(Instructions.Properties));

      var r = GetResult("js");
      r.Contents = t;

      return r;
    }

    private string FormatParameters(IList<ClassMemberStrings> properties)
    {
      var content = GetTextBlock(properties,
        (p) => $"    {p.Parameter}",
        separator: "," + Environment.NewLine);

      return content;
    }

    protected override string FormatProperties(IList<ClassMemberStrings> properties)
    {
      var content = GetTextBlock(properties,
        (p) => $"    this.{p.Parameter} = {p.Parameter};",
        separator: Environment.NewLine);

      return content;
    }
  }
}
