using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.Generators.SqlEngineStrategies;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  /// <summary>
  /// Any templates that are intended to be generated "as-is" with minor or no changes. The rationale
  /// being that providing these files explains the context behind the generated code better, and
  /// will make it easier to test generated code in isolation more immediately. Besides code
  /// this includes documentation to explain how to use the generated code to the user.
  /// </summary>
  public class ImmutableFilesGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.Immutables;

    //TODO: Unfortunately, I am breaking the paradigm of this template generator. I will have to reconsider this later.
    protected override string TemplateName => "MULTIPLE"; //Multiple templates will be used, so this will have to be handled differently.

    protected override string ContainingNamespace => "MULTIPLE"; //Multiple namespaces will be used, so this will have to be handled differently.

    private const string ImmutablesFolderName = "Immutables";

    //TODO: This has to be absorbed by the Architecture classes, specifically for the containing folders
    private readonly Dictionary<string, string> _containingNamespaces = new()
    {
      ["BaseApiController.cs.template"] = "Controllers", //no using directives
      ["BaseManager.cs.template"] = "Managers", //needs using directives
      ["BaseMapper.cs.template"] = "Mappers", //no using directives
      ["BaseRepository.cs.template"] = "DataAccess", //needs using directives
      ["ColumnSchema.cs.template"] = "DataAccess.Utility", //no using directives
      ["DateTimeManager.cs.template"] = "Managers", //no using directives
      ["IAppConfiguration.cs.template"] = string.Empty, //no using directives
      ["IFluentValidation.cs.template"] = "Validation", //no using directives
      ["IRepository.cs.template"] = "DataAccess", //no using directives
      ["UpdateInstruction.cs.template"] = "Managers.Utility" //no using directives
    };

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      /* 2026-01-01
       * For the sake of time, because I need to use this program for a different project very soon,
       * I am going to be only be concerned with SQL Server and asynchronous themed C# for now.
       * I will have to come back to finesse the code later to make it more generic and extensible. */

      //Only for the base repository template at the moment.
      var syntax = BaseSqlEngineSyntax.GetSyntax(instructions.SqlEngine);

      //All templates in the Immutables folder.
      var templates = GetTemplates();

      //Heap of generated results.
      var result = new GeneratedResult(Election, string.Empty, string.Empty);
      result.Heap = new List<GeneratedResult>(templates.Length);

      foreach (var fi in templates)
      {
        var templateName = fi.Name;
        var usingDirectives = new HashSet<string>();

        //Replace just the absolute containing namespace for each template
        var template = new StringBuilder(File.ReadAllText(fi.FullName))
          .Replace("{{AbsoluteContainingNamespace}}", 
            instructions.ArchitectureStrategy.ResolveAbsoluteContainingNamespace(templateName));

        SetAbsoluteContainingNamespace(template, instructions.ArchitectureStrategy);

        SetUsingDirectives(
          template,
          instructions.ArchitectureStrategy,
          usingDirectives,
          ResolutionMethod.Static);

        if (templateName == "BaseRepository.cs.template")
        {
          template.Replace("{{SqlUsingDirectives}}", FormatUsingDirectives(syntax.SqlUsingDirectives));
          template.Replace("{{ConnectionObject}}", syntax.ConnectionObject);
          template.Replace("{{ParameterObject}}", syntax.ParameterObject);
        }

        //Strip the .template extension for the output file. Filename will be named after the template file.
        var fileName = templateName.Replace(".template", string.Empty);

        //TODO: This has to be absorbed by the Architecture classes
        //Get the containing namespace when it exists for the template.
        if (!_containingNamespaces.TryGetValue(templateName, out var containingNamespace))
          containingNamespace = string.Empty; //If it doesn't exist, blank is acceptable for now

        //Add to the heap
        result.Heap.Add(new GeneratedResult(
          Election,
          fileName,
          FormatCSharp(template.ToString()),
          containingNamespace));
      }

      return result;
    }

    private FileInfo[] GetTemplates()
    {
      var path = Path.Combine(TemplatesPath, ImmutablesFolderName);

      if(!Directory.Exists(path)) throw new DirectoryNotFoundException($"The {ImmutablesFolderName} directory was not found: {path}");

      return new DirectoryInfo(path)
        .GetFiles("*.template");
    }
  }
}
