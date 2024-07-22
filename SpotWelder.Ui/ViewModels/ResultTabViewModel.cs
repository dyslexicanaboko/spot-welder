namespace SpotWelder.Ui.ViewModels
{
  public class ResultTabViewModel
    : ObservableObject
  {
    public ResultTabViewModel(string header, string content)
    {
      Header = header;
      Content = content;
    }

    public string Header { get; set; }

    public string Content { get; set; }
  }
}
