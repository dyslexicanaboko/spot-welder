using System.Collections.ObjectModel;
using SpotWelder.Lib.Models.Meta;

namespace SpotWelder.Ui.ViewModels
{
    public class MetaAssemblyViewModel : IMetaAssembly
    {
        public string Name { get; set; }

        public ObservableCollection<MetaClassViewModel> Classes { get; set; }
    }
}
