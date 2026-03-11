using System.Collections.Generic;

namespace SpotWelder.Lib.Services.CodeFactory.ArchitectureStrategy;

public class FeatureBasedArchitectureStrategy(string rootNamespace)
  : ArchitectureStrategyBase(rootNamespace)
{
  protected override Dictionary<GenerationElections, string> ContainingNamespaces { get; set; } = new();

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
