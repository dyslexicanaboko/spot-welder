using System.ComponentModel;
using SpotWelder.Lib.Models.Meta;
using SpotWelder.Ui.ViewModels;

namespace SpotWelder.Ui.Services
{
    public interface IMetaViewModelService
    {
        MetaAssemblyViewModel ToViewModel(MetaAssembly assembly, PropertyChangedEventHandler metaClassCbToggled);
    }
}