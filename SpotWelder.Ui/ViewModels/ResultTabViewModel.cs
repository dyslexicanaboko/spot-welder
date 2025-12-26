namespace SpotWelder.Ui.ViewModels
{
  public class ResultTabViewModel(
    string header,
    string contents,
    string containingFolder) : ObservableObject
  {
    private string _header = header;
    private string _contents = contents;
    private string _containingFolder = containingFolder;

    /// <summary>
    /// Tab header text. Typically, the filename.
    /// </summary>
    public string Header
    {
      get => _header;
      set
      {
        _header = value;

        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Containing folder path for the file content. Used during save.
    /// </summary>
    public string ContainingFolder
    {
      get => _containingFolder;
      set
      {
        _containingFolder = value;

        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Contents to display and ultimately save to a file.
    /// </summary>
    public string Contents
    {
      get => _contents;
      set
      {
        _contents = value;

        OnPropertyChanged();
      }
    }
  }
}
