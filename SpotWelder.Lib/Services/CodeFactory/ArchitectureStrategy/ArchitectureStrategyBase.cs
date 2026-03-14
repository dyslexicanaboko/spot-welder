using System.Collections.Generic;

namespace SpotWelder.Lib.Services.CodeFactory.ArchitectureStrategy;

public abstract class ArchitectureStrategyBase(string rootContainingNamespace)
{
  protected string RootContainingNamespace { get; set; } = rootContainingNamespace;

  protected abstract Dictionary<GenerationElections, string> ContainingNamespaces { get; }
  
  protected abstract Dictionary<string, string> ContainingNamespacesForImmutables { get; }

  protected abstract Dictionary<string, string[]> StaticTemplateUsingDirectives { get; }
  
  protected abstract Dictionary<string, GenerationElections[]> DynamicTemplateUsingDirectives { get; }

  public abstract NamespaceModel ResolveAbsoluteContainingNamespace(GenerationElections election);
  
  public abstract NamespaceModel ResolveAbsoluteContainingNamespace(string immutableTemplateName);
  
  public abstract void ResolveStaticUsingDirectives(HashSet<string> usingDirectives, string templateName);
  
  public abstract void ResolveDynamicUsingDirectives(HashSet<string> usingDirectives, string templateName, GenerationElections elections);

  public abstract ArchitectureStrategyBase Clone();

  protected virtual string Join(string rootContainingNamespace, string containingNamespace)
    => string.IsNullOrWhiteSpace(containingNamespace)
      ? rootContainingNamespace
      : $"{rootContainingNamespace}.{containingNamespace}";
}
