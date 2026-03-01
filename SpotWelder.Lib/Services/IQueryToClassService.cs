using SpotWelder.Lib.Models;
using System.Collections.Generic;

namespace SpotWelder.Lib.Services
{
  public interface IQueryToClassService
  {
    List<GeneratedResult>? Generate(QueryToClassParameters parameters);

    List<GeneratedResult> Generate(DtoInstructions instructions);
  }
}
