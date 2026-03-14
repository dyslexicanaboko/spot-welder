using System.Collections.Generic;

namespace SpotWelder.Lib.Services.CodeFactory.ArchitectureStrategy;

public class FeatureBasedArchitectureStrategy
  : ArchitectureStrategyBase
{
  private const string Subjects = "Subjects";

  public FeatureBasedArchitectureStrategy(string rootContainingNamespace, string subjectName)
    : base(rootContainingNamespace, subjectName)
  {
    var subjectContainingNamespace = Join(Subjects, subjectName);

    ContainingNamespaces = new Dictionary<GenerationElections, string>
    {
      { GenerationElections.Entity, subjectContainingNamespace },
      { GenerationElections.EntityEqualityComparer, subjectContainingNamespace },
      { GenerationElections.Model, subjectContainingNamespace },
      { GenerationElections.CreateModel, subjectContainingNamespace },
      { GenerationElections.PatchModel, subjectContainingNamespace },
      { GenerationElections.CreatedModel, subjectContainingNamespace },
      { GenerationElections.Record, subjectContainingNamespace },
      { GenerationElections.Validation, subjectContainingNamespace },
      { GenerationElections.Mapper, subjectContainingNamespace },
      { GenerationElections.ApiController, "Controllers" },
      { GenerationElections.Manager, subjectContainingNamespace },
      { GenerationElections.RepoDapper, subjectContainingNamespace },
      { GenerationElections.RepoStatic, subjectContainingNamespace },
    };

    ContainingNamespacesForImmutables = new Dictionary<string, string>
    {
      ["BaseApiController.cs.template"] = "Controllers",
      ["BaseManager.cs.template"] = Root,
      ["BaseMapper.cs.template"] = Root,
      ["BaseRepository.cs.template"] = Root,
      ["ColumnSchema.cs.template"] = "Utilities",
      ["DateTimeManager.cs.template"] = Root,
      ["ErrorModel.cs.template"] = "Controllers",
      ["IAppConfiguration.cs.template"] = Root,
      ["IFluentValidation.cs.template"] = Root,
      ["IRepository.cs.template"] = Root,
      ["SerializationService.cs.template"] = Root,
      ["UpdateInstruction.cs.template"] = "Utilities"
    };

    StaticTemplateUsingDirectives = new Dictionary<string, string[]>
    {
      {"ApiController.cs.template", [subjectContainingNamespace]},
      {"ApiControllerReadsOnly.cs.template", [subjectContainingNamespace]},
      {"BaseManager.cs.template", ["Utilities"]},
      {"BaseRepository.cs.template", ["Utilities"]},
    };
  }

  protected override Dictionary<GenerationElections, string> ContainingNamespaces { get; }

  protected override Dictionary<string, string> ContainingNamespacesForImmutables { get; }

  //These templates will always need these namespaces to be imported
  protected override Dictionary<string, string[]> StaticTemplateUsingDirectives { get; }

  //These are the templates that would have dynamic using directives based on elections if it were NTier.
  //In this case the appropriate answer is nothing every time because everything lives in the same `Subjects.SubjectName` namespace always.
  protected override Dictionary<string, GenerationElections[]> DynamicTemplateUsingDirectives => new();

  /// <param name="usingDirectives"></param>
  /// <param name="templateName"></param>
  /// <param name="elections"></param>
  /// <inheritdoc />
  public override void ResolveDynamicUsingDirectives(
    HashSet<string> usingDirectives,
    string templateName,
    GenerationElections elections)
  {
    //The three would be dynamics in this case don't do anything.
    //Read the comment about the DynamicTemplateUsingDirectives property for more info.
  }

  public override ArchitectureStrategyBase Clone() 
    => new FeatureBasedArchitectureStrategy(RootContainingNamespace, SubjectName);
}
