using SpotWelder.Ui.Helpers;
using SpotWelder.Ui.ViewModels;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using Clipboard = System.Windows.Clipboard;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace SpotWelder.Ui
{
  /// <summary>
  /// Parent window for displaying results and managing tabs (formerly individual windows).
  /// This is a successor to <see cref="ResultWindow"/> and the <see cref="ResultWindowManager"/>.
  /// </summary>
  public partial class ParentResultsWindow : Window
  {
    private readonly ParentResultsWindowViewModel _viewModel = new ();

    private static bool _applicationIsShuttingDown = false;

    private ResultTabViewModel SelectedTab => (ResultTabViewModel)TcResults.SelectedItem;

    public ParentResultsWindow()
    {
      InitializeComponent();

      DataContext = _viewModel;
    }

    public void AddTab(string title, string contents)
    {
      _viewModel.Tabs.Add(new ResultTabViewModel(
        title, 
        contents));
    }

    private void BtnCopy_Click(object sender, RoutedEventArgs e)
    {
      Clipboard.SetText(SelectedTab.Content);
    }

    private void BtnSave_OnClick(object sender, RoutedEventArgs e)
    {
      var dlg = new SaveFileDialog();
      dlg.FileName = SelectedTab.Header; // Default file name
      dlg.DefaultExt = ".cs"; // Default file extension

      //dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

      // Show save file dialog box
      var result = dlg.ShowDialog();

      if (result != System.Windows.Forms.DialogResult.OK) return;

      File.WriteAllText(dlg.FileName, SelectedTab.Content);
    }

    //When the user tries to close the window, we want to hide it instead.
    //However, when the application is exiting (shutting down), it needs to close.
    private void ParentResultsWindow_OnClosing(object? sender, CancelEventArgs e)
    {
      if(_applicationIsShuttingDown) return;
      
      // Cancel the close operation
      e.Cancel = true;
      
      //Empty the results tabs
      _viewModel.Tabs.Clear();

      // Hide the window instead
      Hide();
    }

    /// <summary>
    /// Use this in lieu of the stock <see cref="Window.Close"/> method.
    /// </summary>
    public void Shutdown()
    {
      _applicationIsShuttingDown = true;

      Close();
    }

    //Most of the dragging tabs code came from here:
    //https://stackoverflow.com/questions/10738161/is-it-possible-to-rearrange-tab-items-in-tab-control-in-wpf
    private void TabItem_PreviewMouseMove(object sender, MouseEventArgs e)
    {
      if (e.Source is not TabItem tabItem)
      {
        return;
      }

      if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
      {
        DragDrop.DoDragDrop(tabItem, tabItem, DragDropEffects.All);
      }
    }

    private void TabItem_Drop(object sender, DragEventArgs e)
    {
      var a = e.Source is not TabItem;
      var b = e.Data.GetData(typeof(TabItem)) is not TabItem;
      
      if(a || b) return;

      var tabTarget = (ResultTabViewModel)((TabItem)e.Source).DataContext;
      var tabSource = (ResultTabViewModel)((TabItem)e.Data.GetData(typeof(TabItem))).DataContext;

      var c = tabTarget.Equals(tabSource);

      if(c) return;
      
      _viewModel.Tabs.Move(
        _viewModel.Tabs.IndexOf(tabSource), 
        _viewModel.Tabs.IndexOf(tabTarget));
    }

    //In this case, the image is not going to be bound to the view model
    //so instead just use the currently selected tab
    private void TabClose_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
      _viewModel.Tabs.Remove(SelectedTab);
    }

    private void BtnSaveAll_OnClick(object sender, RoutedEventArgs e)
    {
      using var dlg = new FolderBrowserDialog();

      dlg.Description = "Select a folder to save all your files to.";
      dlg.ShowNewFolderButton = true;

      var result = dlg.ShowDialog();

      if (result != System.Windows.Forms.DialogResult.OK) return;

      foreach (var tab in _viewModel.Tabs)
      {
        var filePath = Path.Combine(dlg.SelectedPath, tab.Header + ".cs");

        File.WriteAllText(filePath, tab.Content);
      }

      HlSaveLocation.SetHyperLink(dlg.SelectedPath, dlg.SelectedPath);
    }

    private void HlSaveLocation_OnClick(object sender, RoutedEventArgs e)
    {
      ((Hyperlink)e.OriginalSource).NavigateUri.OpenUri();
    }
  }
}
