using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SpotWelder.Ui.ViewModels
{
  /// <summary>
  /// Container for the tabs.
  /// </summary>
  public class ParentResultsWindowViewModel : ObservableObject
  {
    private ObservableCollection<ResultTabViewModel> _tabs = [];
    /// <summary>
    /// Tabs being managed by the parent window.
    /// </summary>
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
