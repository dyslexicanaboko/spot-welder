using System.Collections.Generic;
using System.Linq;

namespace SpotWelder.Lib.Services.CodeFactory.ArchitectureStrategy;

public abstract class ArchitectureStrategyBase(string rootContainingNamespace, string subjectName)
{
  protected string RootContainingNamespace { get; set; } = rootContainingNamespace;

  protected string SubjectName { get; set; } = subjectName;

  protected const string Root = "";

  /// <summary>
  /// Each individual Generation Election (no flags here), mapped to its containing namespace.
  /// This is not the final absolute containing namespace, it is the suffix.
  /// </summary>
  protected abstract Dictionary<GenerationElections, string> ContainingNamespaces { get; }

  /// <summary>
  /// Individual immutable templates, mapped to their containing namespace.
  /// This is not the final absolute containing namespace, it is the suffix.
  /// </summary>
  protected abstract Dictionary<string, string> ContainingNamespacesForImmutables { get; }

  /// <summary>
  /// Static refers to the idea that these templates always use the same using directives.
  /// Therefore, each specified template name, has a static mapping of one or many using directives.
  /// </summary>
  protected abstract Dictionary<string, string[]> StaticTemplateUsingDirectives { get; }

  /// <summary>
  /// Static refers to the idea that these templates always use the same using directives.
  /// These templates have a static mapping and dependency on one or many using directives from
  /// non-project namespaces (3rd party libraries).
  /// This is separate because we don't want the root namespace to be prepended like it is done
  /// for all other project based using directives.
  /// </summary>
  protected readonly Dictionary<string, string[]> StaticTemplateUsingDirectivesAsIs = new()
  {
    { "BaseRepository.cs.template", ["System.Data", "Dapper"]},
    { "RepositoryDapper.cs.template", ["System.Data", "Dapper"]},
    { "RepositoryDapperReadsOnly.cs.template", ["Dapper"]},
  };

  /// <summary>
  /// Dynamic refers to the idea that these templates have using directives that are based on the Generation Elections provided.
  /// Meaning that the number of using directives varies depending on user elections of <see cref="GenerationElections"/>.
  /// Therefore, the inclusion of using directives is conditional or dynamic.
  /// </summary>
  protected abstract Dictionary<string, GenerationElections[]> DynamicTemplateUsingDirectives { get; }

  /// <summary>
  /// Resolve the absolute containing namespace for the provided Generation Election.
  /// </summary>
  /// <param name="election">Single generation election (not using flags here).</param>
  /// <exception cref="System.ArgumentException">Thrown if the provided Generation Election has multiple flags.</exception>
  /// <returns></returns>
  public virtual NamespaceModel ResolveAbsoluteContainingNamespace(GenerationElections election)
  {
    election.EnsureSingleElection();

    return new NamespaceModel(
      ContainingNamespaces.TryGetValue(election, out var containingNamespace) ?
        Join(RootContainingNamespace, containingNamespace) :
        RootContainingNamespace,
      containingNamespace ?? Root);
  }

  /// <summary>
  /// Resolve the absolute containing namespace for the provided immutable template name.
  /// </summary>
  /// <param name="immutableTemplateName">Name of the immutable template.</param>
  /// <returns></returns>
  public virtual NamespaceModel ResolveAbsoluteContainingNamespace(string immutableTemplateName)
    => new(
      ContainingNamespacesForImmutables.TryGetValue(immutableTemplateName, out var containingNamespace)
        ? Join(RootContainingNamespace, containingNamespace)
        : RootContainingNamespace,
      containingNamespace ?? Root);

  /// <summary>
  /// Resolves and adds static using directives to the specified set based on the provided template name.
  /// </summary>
  /// <remarks>This method first attempts to prepend the root namespace to static using directives associated
  /// with the template name. If the template has static using directives that should remain unchanged, those are added
  /// directly to the set.</remarks>
  /// <param name="usingDirectives">A set of strings that will be populated with the resolved using directives for the specified template.</param>
  /// <param name="templateName">The name of the template for which static using directives are to be resolved.</param>
  public virtual void ResolveStaticUsingDirectives(HashSet<string> usingDirectives, string templateName)
  {
    //These are static template using directives that require the root namespace to be prepended
    if (StaticTemplateUsingDirectives.TryGetValue(templateName, out var staticUsingDirectives))
    {
      usingDirectives.AddRange(staticUsingDirectives.Select(cn => Join(RootContainingNamespace, cn)));
    }

    //These are static template using directives that require the provided namespaces not be modified as they are from 3rd parties (non-project)
    if (!StaticTemplateUsingDirectivesAsIs.TryGetValue(templateName, out staticUsingDirectives)) return;

    usingDirectives.AddRange(staticUsingDirectives);
  }

  /// <summary>
  /// Resolves dynamic using directives for a template based on the specified generation elections.
  /// </summary>
  /// <remarks>Derived classes should implement this method to provide specific resolution logic based on the
  /// template and elections provided.</remarks>
  /// <param name="usingDirectives">A set of using directives to be updated with namespaces or types required for the template.</param>
  /// <param name="templateName">The name of the template for which the using directives are being resolved.</param>
  /// <param name="elections">The generation elections that determine which using directives are applicable for the template.</param>
  public abstract void ResolveDynamicUsingDirectives(HashSet<string> usingDirectives, string templateName, GenerationElections elections);

  /// <summary>
  /// Clones the current instance.
  /// </summary>
  /// <returns></returns>
  public abstract ArchitectureStrategyBase Clone();

  /// <summary>
  /// Joins the specified containing namespace to the root namespace, forming a fully qualified namespace string.
  /// </summary>
  /// <remarks>This method is useful for constructing fully qualified names in scenarios where namespaces are
  /// dynamically determined.</remarks>
  /// <param name="rootContainingNamespace">The root namespace to which the containing namespace will be appended.</param>
  /// <param name="containingNamespace">The namespace to be appended to the root namespace. If this parameter is null or whitespace, the root namespace is
  /// returned unchanged.</param>
  /// <returns>A string representing the fully qualified namespace, which combines the root namespace and the containing
  /// namespace.</returns>
  protected static string Join(string rootContainingNamespace, string containingNamespace)
    => string.IsNullOrWhiteSpace(containingNamespace)
      ? rootContainingNamespace
      : $"{rootContainingNamespace}.{containingNamespace}";
}
