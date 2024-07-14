using SpotWelder.Lib.Models;
using System.Collections.Generic;

namespace SpotWelder.Lib.Services
{
  public interface IQueryToClassService
  {
    IList<GeneratedResult>? Generate(QueryToClassParameters parameters);

    IList<GeneratedResult> Generate(DtoInstructions instructions);
  }
}
