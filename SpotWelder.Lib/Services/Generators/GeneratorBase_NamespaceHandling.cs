using SpotWelder.Lib.Services.CodeFactory.ArchitectureStrategy;
using System.Collections.Generic;
using System.Text;

namespace SpotWelder.Lib.Services.Generators;

/* Everything related to namespaces
 *  Absolute containing namespace
 *  Using directives  */
public partial class GeneratorBase
{
  protected StringBuilder SetAbsoluteContainingNamespace(StringBuilder sb, ArchitectureStrategyBase architectureStrategy, out string containingNamespace)
  {
    var namespaceModel = architectureStrategy.ResolveAbsoluteContainingNamespace(Election);

    //This is needed later for the folder creation
    containingNamespace = namespaceModel.ContainingNamespace;

    return sb.Replace(
      "{{AbsoluteContainingNamespace}}",
      namespaceModel.AbsoluteContainingNamespace);
  }

  protected StringBuilder SetUsingDirectivesStatic(
    StringBuilder sb,
    ArchitectureStrategyBase architectureStrategy,
    HashSet<string> usingDirectives,
    string? overrideTemplateName = null)
  {
    overrideTemplateName ??= TemplateName;

    architectureStrategy.ResolveStaticUsingDirectives(usingDirectives, overrideTemplateName);

    return sb.Replace("{{UsingDirectives}}", FormatUsingDirectives(usingDirectives));
  }

  protected StringBuilder SetUsingDirectivesDynamic(
    StringBuilder sb,
    ArchitectureStrategyBase architectureStrategy,
    HashSet<string> usingDirectives,
    GenerationElections elections,
    string? templateName = null)
  {
    templateName ??= TemplateName;

    architectureStrategy.ResolveDynamicUsingDirectives(usingDirectives, templateName, elections);

    return sb.Replace("{{UsingDirectives}}", FormatUsingDirectives(usingDirectives));
  }
}
