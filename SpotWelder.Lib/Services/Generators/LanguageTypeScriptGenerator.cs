using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.CodeFactory;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class LanguageTypeScriptGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.GenerateEntityAsTypeScript;

    protected override string TemplateName => "Type.ts.template";

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      template.Replace("{{ClassName}}", instructions.EntityName);
      template.Replace("{{Properties}}", FormatProperties(instructions.Properties));

      return new GeneratedResult($"{instructions.SubjectName}.ts", template);
    }

    protected override string FormatProperties(IList<ClassMemberStrings> properties)
    {
      //FYI: The `Parameter` property is being used because it's camelCase
      var content = GetTextBlock(
        properties,
        p => $"    {p.Parameter}{(p.IsDbNullable ? "?" : string.Empty)}: {p.TypeScriptType};",
        Environment.NewLine);

      return content;
    }
  }
}
