namespace SpotWelder.Ui.ViewModels
{
  public class VersionInfoViewModel
    : ObservableObject
  {
    public VersionInfoViewModel(string key, string value)
    {
      Key = key;
      Value = value;
    }

    public string Key { get; set; }
    public string Value { get; set; }
  }
}
