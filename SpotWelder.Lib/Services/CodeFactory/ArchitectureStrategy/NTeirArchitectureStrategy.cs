using System.Collections.Generic;

namespace SpotWelder.Lib.Services.CodeFactory.ArchitectureStrategy;

public class NTeirArchitectureStrategy(string rootNamespace)
  : ArchitectureStrategyBase(rootNamespace)
{
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

  /// <inheritdoc />
  public override string ResolveNamespace()
  {
    return null;
  }

  /// <inheritdoc />
  public override string ResolveUsings()
  {
    return null;
  }
}
