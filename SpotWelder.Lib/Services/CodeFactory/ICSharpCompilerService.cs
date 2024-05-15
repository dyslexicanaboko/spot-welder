using SpotWelder.Lib.Models;

namespace SpotWelder.Lib.Services.CodeFactory
{
    public interface ICSharpCompilerService
    {
        CompilerResult Compile(string classSourceCode);
    }
}