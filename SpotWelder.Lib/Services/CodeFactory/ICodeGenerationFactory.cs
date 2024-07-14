using SpotWelder.Lib.Models;

namespace SpotWelder.Lib.Services.CodeFactory;

public interface ICodeGenerationFactory
{
  GeneratedResult? Generate(ClassInstructions instructions, GenerationElections electionToTest);
}
