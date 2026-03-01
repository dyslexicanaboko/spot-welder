using SpotWelder.Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace SpotWelder.Ui
{
  public static class UserControlExtensions
  {
    public static void ShowErrorMessage(Exception ex) => ShowWarningMessage(ex.Message);

    public static void ShowWarningMessage(string message) => MessageBox.Show(message);

    public static void ShowAsErrorMessage(this Exception ex) => ShowWarningMessage(ex.Message);

    public static void ShowAsWarningMessage(this string message) => ShowWarningMessage(message);

    public static bool IsTextInvalid(
      this TextBoxWithDefaultControl target,
      string message) => IsTextInvalid(target.TextBox, message);

    public static bool IsTextInvalid(this TextBox target, string message)
    {
      var invalid = string.IsNullOrWhiteSpace(target.Text);

      if (invalid)
        ShowWarningMessage(message);

      return invalid;
    }

    public static bool IsCheckedAndEnabled(this CheckBox target) =>
      target.IsEnabled && IsChecked(target);

    public static bool IsChecked(this CheckBox target) => target.IsChecked.GetValueOrDefault();

    public static GenerationElections GetChosenGenerationElections(this Dictionary<GenerationElections, CheckBox> map)
    {
      var e = GenerationElections.None;

      foreach (var kvp in map)
      {
        if (!kvp.Value.IsChecked()) continue;

        e |= kvp.Key;
      }
      
      return e;
    }

    public static void CopyToClipboard(this ContentControl label)
      => Clipboard.SetText(label.Content.ToString() ?? string.Empty);

    public static void CopyToClipboard(this Hyperlink hyperlink, bool isLocalPath = false)
      => Clipboard.SetText(isLocalPath ? hyperlink.NavigateUri.LocalPath : hyperlink.NavigateUri.AbsoluteUri);

    public static void CopyToClipboard(this string content)
      => Clipboard.SetText(content);

    public static void SetHyperLink(this Hyperlink hyperlink, string uri)
      => SetHyperLink(hyperlink, uri, uri);

    public static void SetHyperLink(this Hyperlink hyperlink, string uri, string text)
    {
      hyperlink.NavigateUri = new Uri(string.IsNullOrWhiteSpace(uri) ? "about:blank" : uri);
      hyperlink.Inlines.Clear();
      hyperlink.Inlines.Add(text);
    }

    public static void OpenUri(this Hyperlink hyperlink)
      => OpenUri(hyperlink.NavigateUri);

    public static void OpenUri(this Uri uri)
      => OpenUri(uri.ToString());

    /// <summary>
    ///   Open a URL or a folder path.
    /// </summary>
    /// <param name="uri"></param>
    public static void OpenUri(this string uri)
      => Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });

    //https://stackoverflow.com/questions/876473/is-there-a-way-to-check-if-a-file-is-in-use
    public static bool IsFileLocked(string fullFilePath)
    {
      try
      {
        using var stream = new FileInfo(fullFilePath).Open(FileMode.Open, FileAccess.Read, FileShare.None);

        stream.Close();
      }
      catch (IOException)
      {
        //the file is unavailable because it is:
        //still being written to
        //or being processed by another thread
        //or does not exist (has already been processed)
        return true;
      }

      //file is not locked
      return false;
    }

    //TODO: Until there is a better way to manage window positioning, this will do.
    // I am sure there is a more appropriate way to do this in WPF.
    /// <summary>
    /// Configures the window to be centered on the main application window.
    /// </summary>
    /// <param name="window">The window to configure.</param>
    public static void PositionChildWindowOnActiveMonitor(this Window window)
    {
      if (!window.IsVisible) throw new ApplicationException("This method cannot be called unless the window is visible.");

      window.Owner = Application.Current.MainWindow;
      window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
    }

    /// <summary>
    /// Positions the specified window at the center of the monitor that currently contains the mouse pointer.
    /// I was tired of not having the window on my active monitor. I doubt this is the right way to do this, but
    /// it works for now. I did the same thing to the child windows using <see cref="PositionChildWindowOnActiveMonitor"/>.
    /// </summary>
    /// <remarks>This method sets the window's startup location to manual and moves it so that it is centered
    /// within the working area of the monitor where the mouse pointer is currently located. This is useful for ensuring
    /// dialogs or windows appear on the user's active screen in multi-monitor setups.</remarks>
    /// <param name="window">The window to position on the active monitor. Cannot be null.</param>
    public static void PositionWindowOnActiveMonitor(this Window window)
    {
      // Get the current mouse position
      var mousePosition = System.Windows.Forms.Control.MousePosition;

      // Get the screen where the mouse is located
      var screen = System.Windows.Forms.Screen.FromPoint(mousePosition);

      // Set window position to manual so we can control it
      window.WindowStartupLocation = WindowStartupLocation.Manual;

      // Calculate center position on the screen with the mouse
      window.Left = screen.WorkingArea.Left + (screen.WorkingArea.Width - window.Width) / 2;
      window.Top = screen.WorkingArea.Top + (screen.WorkingArea.Height - window.Height) / 2;
    }
  }
}
