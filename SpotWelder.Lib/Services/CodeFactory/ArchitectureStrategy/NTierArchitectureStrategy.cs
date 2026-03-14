using System.Collections.Generic;
using System.Linq;

namespace SpotWelder.Lib.Services.CodeFactory.ArchitectureStrategy;

public class NTierArchitectureStrategy(string rootContainingNamespace)
  : ArchitectureStrategyBase(rootContainingNamespace)
{
  //This has to remain 1:1
  protected override Dictionary<GenerationElections, string> ContainingNamespaces => new()
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

  /// <inheritdoc />
  protected override Dictionary<string, string> ContainingNamespacesForImmutables => new()
  {
    ["BaseApiController.cs.template"] = "Controllers",
    ["BaseManager.cs.template"] = "Managers",
    ["BaseMapper.cs.template"] = "Mappers",
    ["BaseRepository.cs.template"] = "DataAccess",
    ["ColumnSchema.cs.template"] = "DataAccess.Utility",
    ["DateTimeManager.cs.template"] = "Managers",
    ["ErrorModel.cs.template"] = "Controllers",
    ["IAppConfiguration.cs.template"] = string.Empty,
    ["IFluentValidation.cs.template"] = "Validation",
    ["IRepository.cs.template"] = "DataAccess",
    ["UpdateInstruction.cs.template"] = "Managers.Utility"
  };

  //These templates will always need these namespaces to be imported
  protected override Dictionary<string, string[]> StaticTemplateUsingDirectives => new()
  {
    {"ApiController.cs.template", ["Managers", "Mappers", "Models", "Models.Client"]},
    {"ApiControllerReadsOnly.cs.template", ["Managers", "Mappers", "Models"]},
    {"BaseManager.cs.template", ["Managers.Utility"]},
    {"BaseRepository.cs.template", ["DataAccess.Utility", "Managers.Utility", "System.Data", "Dapper"]},
    {"EntityEqualityComparer.cs.template", ["Entities"]},
    {"EntityValidation.cs.template", ["Entities"]},
    {"IManager.cs.template", ["Entities"]},
    {"IRepositoryDapper.cs.template", ["Records"]},
    {"Manager.cs.template", ["DataAccess", "Entities", "Mappers", "Validation"]},
    {"ManagerReadsOnly.cs.template", ["DataAccess", "Entities", "Mappers", "Validation"]},
    {"ModelCreated.cs.template", ["Entities"]},
    {"ModelPatch.cs.template", ["Entities"]},
    {"RepositoryDapper.cs.template", ["Entities", "System.Data", "Dapper"]},
    {"RepositoryDapperReadsOnly.cs.template", ["Entities", "System.Data", "Dapper"]},
    {"ServiceSerializationCsv.cs.template", ["Entities"]},
    {"ServiceSerializationJson.cs.template", ["Entities"]},
  };

  //These are the templates that have dynamic using directives based on elections.
  protected override Dictionary<string, GenerationElections[]> DynamicTemplateUsingDirectives => new()
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
    ]},
    {"Model.cs.template", [
      GenerationElections.Entity,
    ]},
  };

  //Cross-reference for dynamic using directives. If an election is present, then the
  //dependent namespaces will be added as using directives.
  private readonly Dictionary<GenerationElections, string[]> _usingDirectives = new()
  {
    { GenerationElections.CreateModel, ["Models.Client"] },
    { GenerationElections.Entity, ["Entities"] },
    { GenerationElections.MapCreateModelToEntity, ["Entities", "Models.Client"] },
    { GenerationElections.MapEntityToCreatedModel, ["Entities", "Models.Client"] },
    { GenerationElections.MapEntityToModel, ["Entities", "Models"] },
    { GenerationElections.MapEntityToRecord, ["Entities", "Records"] },
    { GenerationElections.MapModelToEntity, ["Entities", "Models"] },
    { GenerationElections.MapPatchModelToEntity, ["Entities", "Models.Client"] },
    { GenerationElections.MapRecordToEntity, ["Entities", "Records"] },
    { GenerationElections.Model, ["Models"] },
    { GenerationElections.PatchModel, ["Models.Client"] },
    { GenerationElections.Record, ["Records"] },
  };

  /// <param name="election"></param>
  /// <inheritdoc />
  public override NamespaceModel ResolveAbsoluteContainingNamespace(GenerationElections election)
    => new(
      ContainingNamespaces.TryGetValue(election, out var containingNamespace)
      ? Join(RootContainingNamespace, containingNamespace)
      : RootContainingNamespace, 
      containingNamespace ?? string.Empty);

  public override NamespaceModel ResolveAbsoluteContainingNamespace(string immutableTemplateName)
    => new (
      ContainingNamespacesForImmutables.TryGetValue(immutableTemplateName, out var containingNamespace)
      ? Join(RootContainingNamespace, containingNamespace)
      : RootContainingNamespace,
      containingNamespace ?? string.Empty);

  public override void ResolveStaticUsingDirectives(HashSet<string> usingDirectives, string templateName)
  {
    if(!StaticTemplateUsingDirectives.TryGetValue(templateName, out var staticUsingDirectives)) return;

    usingDirectives.AddRange(staticUsingDirectives.Select(cn => Join(RootContainingNamespace, cn)));
  }

  //Each generator already has the logic baked in on when to add a namespace for NTier
  //so, the only thing this method will do is construct the using.
  /// <param name="usingDirectives"></param>
  /// <param name="templateName"></param>
  /// <param name="elections"></param>
  /// <inheritdoc />
  public override void ResolveDynamicUsingDirectives(
    HashSet<string> usingDirectives,
    string templateName,
    GenerationElections elections)
  {
    if(!DynamicTemplateUsingDirectives.TryGetValue(templateName, out var dynamicNamespaces)) return;
    
    foreach (var election in dynamicNamespaces)
    {
      if (!elections.HasFlag(election)) continue;

      usingDirectives.AddRange(ResolveUsingDirectives(election));
    }
  }

  private string[] ResolveUsingDirectives(GenerationElections election)
    => _usingDirectives.TryGetValue(election, out var usingDirectives)
      ? usingDirectives.Select(cn => Join(RootContainingNamespace, cn)).ToArray()
      : [];

  public override ArchitectureStrategyBase Clone() 
    => new NTierArchitectureStrategy(RootContainingNamespace);
}
