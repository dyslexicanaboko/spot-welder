using System;
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
  }
}
