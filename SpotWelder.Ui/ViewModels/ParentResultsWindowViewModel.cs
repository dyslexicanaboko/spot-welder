using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SpotWelder.Ui.ViewModels
{
  public class ParentResultsWindowViewModel
  {
    public ObservableCollection<ResultTabViewModel> Tabs { get; set; } = new();

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
