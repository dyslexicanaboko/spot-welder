using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.Generators;
using System.Collections.Generic;
using System.Linq;

namespace SpotWelder.Lib.Services.CodeFactory
{
  /// <summary>
  /// Handling the user's elections. All generators are mapped to an election.
  /// </summary>
  public class CodeGenerationFactory
    : ICodeGenerationFactory
  {
    private readonly Dictionary<GenerationElections, GeneratorBase> _generators;

    /// <summary>All classes that extend the <see cref="GeneratorBase"/> will be initialized and turned into an enumerable.</summary>
    public CodeGenerationFactory(IEnumerable<GeneratorBase> generators)
    {
      _generators = generators.ToDictionary(g => g.Election);
    }

    public GeneratedResult? Generate(ClassInstructions instructions, GenerationElections electionToTest)
    {
      //If the user did not make this election, then nothing will be generated.
      if (!instructions.Elections.HasFlag(electionToTest)) return null;
      
      return _generators[electionToTest].FillTemplate(instructions.Clone());
    }
  }
}
