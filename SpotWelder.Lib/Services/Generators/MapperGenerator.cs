﻿using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.CodeFactory;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  public class MapperGenerator
    : GeneratorBase
  {
    private readonly ClassServices _services;

    public MapperGenerator(ClassInstructions instructions, ClassServices services)
      : base(instructions, "Mapper.cs.template")
      => _services = services;

    public override GeneratedResult FillTemplate()
    {
      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      //Instructions.InterfaceName

      template.Replace("{{Namespace}}", Instructions.Namespace);
      template.Replace("{{EntityName}}", Instructions.EntityName);
      template.Replace("{{ModelName}}", Instructions.ModelName);
      template.Replace("{{InterfaceName}}", Instructions.InterfaceName);
      template.Replace("{{Namespaces}}", FormatNamespaces(Instructions.Namespaces));

      //Cloning method properties
      template.Replace(
        "{{PropertiesModelToEntity}}",
        FormatCloneBody(
          ClassServices.CloneModelToEntity,
          Instructions.Properties,
          "model",
          "entity"));

      template.Replace(
        "{{PropertiesEntityToModel}}",
        FormatCloneBody(
          ClassServices.CloneEntityToModel,
          Instructions.Properties,
          "entity",
          "model"));

      template.Replace(
        "{{PropertiesInterfaceToEntity}}",
        FormatCloneBody(
          ClassServices.CloneInterfaceToEntity,
          Instructions.Properties,
          "target",
          "entity"));

      template.Replace(
        "{{PropertiesInterfaceToModel}}",
        FormatCloneBody(
          ClassServices.CloneInterfaceToModel,
          Instructions.Properties,
          "target",
          "model"));

      var t = template.ToString();

      t = RemoveExcessBlankSpace(t);

      var r = GetResult();
      r.Filename = "Mapper.cs";
      r.Contents = t;

      return r;
    }

    private string FormatCloneBody(
      ClassServices flag,
      IList<ClassMemberStrings> properties,
      string from,
      string to)
    {
      if (!_services.HasFlag(flag))
      {
        var exception = GetNotImplementedException(
          $"Cloning option \"{flag}\" was excluded from generation. Delete this method.");

        return exception;
      }

      var content = GetTextBlock(
        properties,
        p => $"			{to}.{p.Property} = {from}.{p.Property};",
        Environment.NewLine);

      return content;
    }
  }
}
