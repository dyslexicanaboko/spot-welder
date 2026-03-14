using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.CodeFactory;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpotWelder.Lib.Services.Generators;

public class LanguageTypeScriptGenerator
  : GeneratorBase
{
  public override GenerationElections Election => GenerationElections.EntityAsTypeScript;

  protected override string TemplateName => "Type.ts.template";

  public override GeneratedResult FillTemplate(ClassInstructions instructions)
  {
    var strTemplate = GetTemplate(TemplateName);

    var template = new StringBuilder(strTemplate);

    template.Replace("{{ClassName}}", instructions.EntityName);
    template.Replace("{{Properties}}", FormatProperties(instructions.Properties));

    return new GeneratedResult(Election, $"{instructions.SubjectName}.ts", template);
  }

  //FYI: The `Parameter` property is being used because it's camelCase
  protected override string FormatProperties(List<ClassMemberStrings> properties)
    => GetTextBlock(
      properties,
      p => $"    {p.Parameter}{(p.IsDbNullable ? "?" : string.Empty)}: {p.TypeScriptType};",
      Environment.NewLine);
}