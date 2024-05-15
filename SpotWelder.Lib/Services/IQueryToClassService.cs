using System.Collections.Generic;
using SpotWelder.Lib.Models;

namespace SpotWelder.Lib.Services
{
    public interface IQueryToClassService
    {
        IList<GeneratedResult> Generate(QueryToClassParameters parameters);

        IList<GeneratedResult> Generate(DtoInstructions parameters);
    }
}