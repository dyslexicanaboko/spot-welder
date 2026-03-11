using System.Collections.Generic;

namespace SpotWelder.Lib.Services.CodeFactory.ArchitectureStrategy;

public class FeatureBasedArchitectureStrategy(string rootNamespace)
  : ArchitectureStrategyBase(rootNamespace)
{
  protected override Dictionary<GenerationElections, string> ContainingNamespaces { get; set; } = new();

  /// <inheritdoc />
  protected override Dictionary<string, string[]> StaticTemplateUsings { get; set; } = new();

  protected override Dictionary<string, GenerationElections[]> ConditionalTemplateUsings { get; set; } = new();

  /// <param name="election"></param>
  /// <inheritdoc />
  public override string ResolveNamespace(GenerationElections election)
    => RootNamespace; //Requires the subject too...

  public override void ResolveStaticUsings(ref List<string> namespaces, string templateName)
  {

  }

  /// <param name="namespaces"></param>
  /// <param name="templateName"></param>
  /// <param name="elections"></param>
  /// <inheritdoc />
  public override void ResolveConditionalUsings(
    ref List<string> namespaces,
    string templateName,
    GenerationElections elections)
  {
    //This is going to be confusing for Feature Based...
    //In some cases you add nothing like for Entity
    //But for anything that utilizes a base class it's going to be weird.
  }
}
