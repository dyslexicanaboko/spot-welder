using SpotWelder.Lib;
using SpotWelder.Ui.Helpers;
using SpotWelder.Ui.Models;
using SpotWelder.Ui.ViewModels;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

    public static readonly RoutedUICommand SaveAll = new (
      "Save All", // Display text
      "SaveAll",  // Command name
      typeof(ParentResultsWindow),
      new InputGestureCollection
      {
        new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift) // Ctrl+Shift+S
      });

    private static bool _applicationIsShuttingDown;

    private ResultTabViewModel SelectedTab => (ResultTabViewModel)TcResults.SelectedItem;

    /// <summary>
    /// Conduit for reporting errors to a parent window.
    /// This helps avoid passing the parent's logger to the child window.
    /// </summary>
    public event EventHandler<ChildErrorEventArgs>? ErrorOccurred;

    public ParentResultsWindow()
    {
      InitializeComponent();
      
      DataContext = _viewModel;
    }

    /// <summary>
    /// Report the error up to the parent window so it can be handled.
    /// </summary>
    /// <param name="exception">Error that has occurred.</param>
    /// <param name="message">Additional information is any.</param>
    protected virtual void ReportError(Exception exception, string message = "")
      => ErrorOccurred?.Invoke(this, new ChildErrorEventArgs(exception, message));

    public void AddTab(string title, string contents, string containingFolder)
      => _viewModel.Tabs.Add(new ResultTabViewModel(
        title, 
        contents,
        containingFolder));

    public void ShowOnActiveWindow()
    {
      this.PositionWindowOnActiveMonitor();
      
      Show();
     
      //this.ConfigureChildWindowPosition(); //Doesn't work in this situation
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

    private void BtnCopy_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        Clipboard.SetDataObject(SelectedTab.Contents);
      }
      catch (COMException cex)
      {
        UserControlExtensions.ShowWarningMessage($"Clipboard appears to be unavailable. Please try again in a moment.\r\nError: {cex.Message}");

        ReportError(cex, "Clipboard is unavailable.");
      }
      catch (Exception ex)
      {
        ex.ShowAsErrorMessage();
       
        ReportError(ex, "Unexpected error.");
      }
    }

    private void SaveOneFile()
    {
      var dlg = new SaveFileDialog();
      dlg.FileName = SelectedTab.Header; // Default file name
      dlg.DefaultExt = ".cs"; // Default file extension

      //dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

      // Show save file dialog box
      var result = dlg.ShowDialog();

      if (result != System.Windows.Forms.DialogResult.OK) return;

      Utils.WriteFile(dlg.FileName, SelectedTab.Contents);
    }

    private void BtnSave_OnClick(object sender, RoutedEventArgs e)
      => SaveOneFile();

    private void SaveAllFilesToFolder()
    {
      using var dlg = new FolderBrowserDialog();

      dlg.Description = "Select a folder to save all your files to.";
      dlg.ShowNewFolderButton = true;

      var result = dlg.ShowDialog();

      if (result != System.Windows.Forms.DialogResult.OK) return;

      foreach (var tab in _viewModel.Tabs)
      {
        var paths = tab.ContainingFolder.Split('.').ToList();

        paths.Insert(0, dlg.SelectedPath);
        paths.Add(tab.Header);

        var fullFilePath = Path.Combine(paths.ToArray());

        //Ensure the lineage of directories exists. They will be created if they don't exist only.
        Directory.CreateDirectory(Path.GetDirectoryName(fullFilePath)!);

        Utils.WriteFile(fullFilePath, tab.Contents);
      }

      HlSaveLocation.SetHyperLink(dlg.SelectedPath, dlg.SelectedPath);
    }

    private void BtnSaveAll_OnClick(object sender, RoutedEventArgs e)
      => SaveAllFilesToFolder();

    private void HlSaveLocation_OnClick(object sender, RoutedEventArgs e)
    {
      ((Hyperlink)e.OriginalSource).NavigateUri.OpenUri();
    }

    private void CommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      if (e.Command == ApplicationCommands.Save)
      {
        SaveOneFile();
      }
      else if (e.Command == SaveAll)
      {
        SaveAllFilesToFolder();
      }
    }
  }
}
