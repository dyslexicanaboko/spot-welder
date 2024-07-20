using System;

namespace SpotWelder.Lib.Services.Generators.Elections
{
  public class GenerationElectionChildAttribute : Attribute
  {
    public GenerationElections Parent { get; }

    public GenerationElectionChildAttribute(GenerationElections parent)
    {
      Parent = parent;
    }
  }
}
