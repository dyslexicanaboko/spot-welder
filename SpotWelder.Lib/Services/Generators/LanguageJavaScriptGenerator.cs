using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.CodeFactory;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class LanguageJavaScriptGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.GenerateEntityAsJavaScript;

    protected override string TemplateName => "Prototype.js.template";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      template.Replace("{{ClassName}}", instructions.EntityName);

      var t = template.ToString();

      t = RemoveExcessBlankSpace(t);

      //FYI: The `Parameter` property is being used because it's camelCase
      t = t.Replace("{{Parameters}}", FormatParameters(instructions.Properties));
      t = t.Replace("{{Properties}}", FormatProperties(instructions.Properties));

      var r = GetResult("js");
      r.Contents = t;

      return r;
    }

    private string FormatParameters(IList<ClassMemberStrings> properties)
    {
      var content = GetTextBlock(
        properties,
        p => $"    {p.Parameter}",
        "," + Environment.NewLine);

      return content;
    }

    protected override string FormatProperties(IList<ClassMemberStrings> properties)
    {
      var content = GetTextBlock(
        properties,
        p => $"    this.{p.Parameter} = {p.Parameter};",
        Environment.NewLine);

      return content;
    }
  }
}
