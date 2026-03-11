using System.Collections.Generic;

namespace SpotWelder.Lib.Services.CodeFactory.ArchitectureStrategy;

public abstract class ArchitectureStrategyBase(string rootNamespace)
{
  protected string RootNamespace { get; set; } = rootNamespace;

  protected abstract Dictionary<GenerationElections, string> ContainingNamespaces { get; set; }

  protected abstract Dictionary<string, string[]> StaticTemplateUsings { get; set; }
  
  protected abstract Dictionary<string, GenerationElections[]> ConditionalTemplateUsings { get; set; }

  public abstract string ResolveNamespace(GenerationElections election);
  
  public abstract void ResolveStaticUsings(ref List<string> namespaces, string templateName);
  
  public abstract void ResolveConditionalUsings(ref List<string> namespaces, string templateName, GenerationElections elections);
}
