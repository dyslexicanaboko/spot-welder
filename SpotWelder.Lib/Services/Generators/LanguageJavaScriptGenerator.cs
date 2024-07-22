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
      
      //FYI: The `Parameters` property is being used because it's camelCase
      template.Replace("{{Parameters}}", FormatParameters(instructions.Properties));
      template.Replace("{{Properties}}", FormatProperties(instructions.Properties));

      return new GeneratedResult($"{instructions.SubjectName}.js", template);
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
