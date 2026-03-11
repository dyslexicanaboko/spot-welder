using System.Collections.Generic;

namespace SpotWelder.Lib.Services.CodeFactory.ArchitectureStrategy;

public abstract class ArchitectureStrategyBase(string rootNamespace)
{
  protected string RootNamespace { get; set; } = rootNamespace;

  protected abstract Dictionary<GenerationElections, string> ContainingNamespaces { get; set; }

  public abstract string ResolveNamespace();
  
  public abstract string ResolveUsings();

}
