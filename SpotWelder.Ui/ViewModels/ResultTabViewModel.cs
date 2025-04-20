namespace SpotWelder.Ui.ViewModels
{
  public class ResultTabViewModel(string header, string content) : ObservableObject
  {
    private string _header = header;
    private string _content = content;

    public string Header
    {
      get => _header;
      set
      {
        _header = value;

        OnPropertyChanged();
      }
    }

    public string Content
    {
      get => _content;
      set
      {
        _content = value;

        OnPropertyChanged();
      }
    }
  }
}
