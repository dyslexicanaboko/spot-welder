using System.Collections.Generic;

namespace SpotWelder.Lib.Services.CodeFactory.ArchitectureStrategy;

public class FeatureBasedArchitectureStrategy(string rootContainingNamespace)
  : ArchitectureStrategyBase(rootContainingNamespace)
{
  protected override Dictionary<GenerationElections, string> ContainingNamespaces { get; } = new();

  protected override Dictionary<string, string> ContainingNamespacesForImmutables { get; } = new();

  /// <inheritdoc />
  protected override Dictionary<string, string[]> StaticTemplateUsingDirectives { get; } = new();

  protected override Dictionary<string, GenerationElections[]> DynamicTemplateUsingDirectives { get; } = new();

  /// <param name="election"></param>
  /// <inheritdoc />
  public override string ResolveAbsoluteContainingNamespace(GenerationElections election)
    => RootContainingNamespace; //Requires the subject too...

  public override string ResolveAbsoluteContainingNamespace(string immutableTemplateName)
    => RootContainingNamespace; //Requires the subject too...

  public override void ResolveStaticUsingDirectives(HashSet<string> usingDirectives, string templateName)
  {

  }

  /// <param name="usingDirectives"></param>
  /// <param name="templateName"></param>
  /// <param name="elections"></param>
  /// <inheritdoc />
  public override void ResolveDynamicUsingDirectives(
    HashSet<string> usingDirectives,
    string templateName,
    GenerationElections elections)
  {
    //This is going to be confusing for Feature Based...
    //In some cases you add nothing like for Entity
    //But for anything that utilizes a base class it's going to be weird.
  }

  public override ArchitectureStrategyBase Clone() 
    => new FeatureBasedArchitectureStrategy(RootContainingNamespace);
}
