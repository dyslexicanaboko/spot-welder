using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SpotWelder.Ui.ViewModels
{
  public class ParentResultsWindowViewModel : ObservableObject
  {
    private ObservableCollection<ResultTabViewModel> _tabs = [];
    public ObservableCollection<ResultTabViewModel> Tabs 
    { 
      get => _tabs;
      set
      {
        _tabs = value;
        
        OnPropertyChanged();
      }
    }

    public ICommand CloseTabCommand { get; }

    public ParentResultsWindowViewModel()
    {
      CloseTabCommand = new RelayCommand<ResultTabViewModel>(CloseTab);
    }

    private void CloseTab(ResultTabViewModel tab)
    {
      Tabs.Remove(tab);
    }
  }
}
