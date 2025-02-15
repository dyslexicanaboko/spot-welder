using SpotWelder.Lib.Services;
using SpotWelder.Lib.Services.CodeFactory;
using SpotWelder.Ui.Services;

namespace SpotWelder.Ui.Controls
{
  public class DtoMakerControlDependencies
  {
    public DtoMakerControlDependencies(
      IDtoGenerator dtoGenerator,
      IMetaViewModelService metaViewModelService,
      IQueryToClassService queryToClassService,
      ICSharpCompilerService compilerService)
    {
      DtoGenerator = dtoGenerator;
      QueryToClassService = queryToClassService;
      MetaViewModelService = metaViewModelService;
      QueryToClassService = queryToClassService;
      CompilerService = compilerService;
    }

    public IDtoGenerator DtoGenerator { get; set; }

    public IMetaViewModelService MetaViewModelService { get; set; }

    public IQueryToClassService QueryToClassService { get; set; }

    public ICSharpCompilerService CompilerService { get; set; }
  }
}
