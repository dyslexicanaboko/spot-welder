namespace SpotWelder.Lib.Services.CodeFactory.ArchitectureStrategy;

public class NamespaceModel(string absoluteContainingNamespace, string containingNamespace)
{
  public string AbsoluteContainingNamespace { get; set; } = absoluteContainingNamespace;

  public string ContainingNamespace { get; set; } = containingNamespace;
}
