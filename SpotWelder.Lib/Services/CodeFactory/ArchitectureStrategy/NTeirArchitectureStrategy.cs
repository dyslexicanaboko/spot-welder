using System.Collections.Generic;
using System.Linq;

namespace SpotWelder.Lib.Services.CodeFactory.ArchitectureStrategy;

public class NTeirArchitectureStrategy(string rootNamespace)
  : ArchitectureStrategyBase(rootNamespace)
{
  //This has to remain 1:1
  protected override Dictionary<GenerationElections, string> ContainingNamespaces { get; set; } = new()
  {
    { GenerationElections.Entity, "Entities" },
    { GenerationElections.Model, "Models" },
    { GenerationElections.CreateModel, "Models.Client" },
    { GenerationElections.PatchModel, "Models.Client" },
    { GenerationElections.CreatedModel, "Models.Client" },
    { GenerationElections.Record, "Records" },
    { GenerationElections.Validation, "Validation" },
    { GenerationElections.Mapper, "Mappers" },
    { GenerationElections.ApiController, "Controllers" },
    { GenerationElections.Manager, "Managers" },
    { GenerationElections.RepoDapper, "DataAccess" },
    { GenerationElections.RepoStatic, "DataAccess" },
  };

  private readonly Dictionary<GenerationElections, string[]> _usings = new()
  {
    { GenerationElections.CreateModel, ["Models.Client"] },
    { GenerationElections.Model, ["Models"] },
    { GenerationElections.PatchModel, ["Models.Client"] },
    { GenerationElections.Record, ["Records"] },
    { GenerationElections.MapEntityToModel, ["Entities", "Models"] },
    { GenerationElections.MapModelToEntity, ["Entities", "Models"] },
    { GenerationElections.MapCreateModelToEntity, ["Entities", "Models.Client"] },
    { GenerationElections.MapPatchModelToEntity, ["Entities", "Models.Client"] },
    { GenerationElections.MapEntityToCreatedModel, ["Entities", "Models.Client"] },
    { GenerationElections.MapRecordToEntity, ["Entities", "Records"] },
    { GenerationElections.MapEntityToRecord, ["Entities", "Records"] }
  };

  protected override Dictionary<string, string[]> StaticTemplateUsings { get; set; } = new()
  {
    {"ApiController.cs.template", ["Managers", "Mappers", "Models", "Models.Client"]},
    {"ApiControllerReadsOnly.cs.template", ["Managers", "Mappers", "Models"]},
    {"EntityValidation.cs.template", ["Entities"]},
    {"Manager.cs.template", ["DataAccess", "Entities", "Mappers", "Validation"]},
    {"ManagerReadsOnly.cs.template", ["DataAccess", "Entities", "Mappers", "Validation"]},
  };

  protected override Dictionary<string, GenerationElections[]> ConditionalTemplateUsings { get; set; } = new()
  {
    {"Entity.cs.template", [
      GenerationElections.Model,
      GenerationElections.CreateModel,
      GenerationElections.PatchModel,
      GenerationElections.Record
    ]},
    {"Mapper.cs.template", [
      GenerationElections.MapEntityToModel,
      GenerationElections.MapModelToEntity,
      GenerationElections.MapCreateModelToEntity,
      GenerationElections.MapPatchModelToEntity,
      GenerationElections.MapEntityToCreatedModel,
      GenerationElections.MapRecordToEntity,
      GenerationElections.MapEntityToRecord,
    ]}
  };

  /// <param name="election"></param>
  /// <inheritdoc />
  public override string ResolveNamespace(GenerationElections election)
    => ContainingNamespaces.TryGetValue(election, out var ns)
      ? $"{RootNamespace}.{ns}"
      : RootNamespace;

  public override void ResolveStaticUsings(ref List<string> namespaces, string templateName)
  {
    if(!StaticTemplateUsings.TryGetValue(templateName, out var staticNamespaces)) return;

    namespaces.AddRange(staticNamespaces.Select(ns => $"{RootNamespace}.{ns}"));
  }

  //Each generator already has the logic baked in on when to add a namespace for NTeir
  //so, the only thing this method will do is construct the using.
  /// <param name="namespaces"></param>
  /// <param name="templateName"></param>
  /// <param name="elections"></param>
  /// <inheritdoc />
  public override void ResolveConditionalUsings(
    ref List<string> namespaces,
    string templateName,
    GenerationElections elections)
  {
    if(!ConditionalTemplateUsings.TryGetValue(templateName, out var conditionalNamespaces)) return;
    
    foreach (var election in conditionalNamespaces)
    {
      if (!elections.HasFlag(election)) continue;

      namespaces.AddRange(ResolveUsings(election));
    }
  }

  private string[] ResolveUsings(GenerationElections election)
    => _usings.TryGetValue(election, out var namespaces)
      ? namespaces.Select(ns => $"{RootNamespace}.{ns}").ToArray()
      : [];
}
