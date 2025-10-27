using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SpotWelder.Ui
{
  /// <summary>
  ///   Interaction logic for ResultWindow.xaml
  /// </summary>
  public partial class ResultWindow : Window
  {
    public ResultWindow(string title, string contents)
    {
      InitializeComponent();

      Title = title;

      TxtResult.Text = contents;

      LineNumbers.Text = GetLineNumbers(contents);
    }

    private static string GetLineNumbers(string input)
    {
      if (string.IsNullOrEmpty(input))
      {
        return "01";
      }

      // Split the string by newline characters
      var lines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

      return string.Join(Environment.NewLine, Enumerable.Range(1, lines.Length).Select(i => i.ToString("00")));
    }

    private void BtnCopy_Click(object sender, RoutedEventArgs e)
    {
      Clipboard.SetText(TxtResult.Text);
    }

    private void BtnSave_OnClick(object sender, RoutedEventArgs e)
    {
      var dlg = new SaveFileDialog();
      dlg.FileName = Title; // Default file name
      dlg.DefaultExt = ".cs"; // Default file extension

      //dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

      // Show save file dialog box
      var result = dlg.ShowDialog().GetValueOrDefault();

      if (!result) return;

      File.WriteAllText(dlg.FileName, TxtResult.Text);
    }

    private void TxtGoToLine_OnKeyDown(object sender, KeyEventArgs e)
    {
      //If it's not the enter key ignore it
      if (e.Key != Key.Enter) return;

      //If it's not a number ignore it
      if (!int.TryParse(TxtGoToLine.Text, out var lineNumber)) return;

      try
      {
        HighlightLineIndex(lineNumber - 1);

        e.Handled = true;
      }
      catch (Exception exception)
      {
        exception.ShowAsErrorMessage();
      }
    }

    private void TxtFind_OnKeyDown(object sender, KeyEventArgs e)
    {
      //If it's not the enter key ignore it
      if (e.Key != Key.Enter) return;

      //If it's blank as in string.Empty ignore it
      if (string.IsNullOrEmpty(TxtSearchText.Text)) return;

      try
      {
        var charIndex = TxtResult.Text.IndexOf(TxtSearchText.Text, StringComparison.OrdinalIgnoreCase);

        HighlightLineIndex(
          TxtResult.GetLineIndexFromCharacterIndex(charIndex),
          charIndex,
          TxtSearchText.Text.Length);

        e.Handled = true;
      }
      catch (Exception exception)
      {
        exception.ShowAsErrorMessage();
      }
    }

    //Default behavior is to highlight the whole row
    private void HighlightLineIndex(int lineIndex, int highlightStartIndex = 0, int highlightLength = 0)
    {
      if (lineIndex < 0 || lineIndex > TxtResult.LineCount) return;

      if (highlightStartIndex <= 0) highlightStartIndex = TxtResult.GetCharacterIndexFromLineIndex(lineIndex);

      if (highlightLength <= 0) highlightLength = TxtResult.GetLineLength(lineIndex);
      
      TxtResult.ScrollToLine(lineIndex);
      TxtResult.CaretIndex = lineIndex;
      TxtResult.Select(highlightStartIndex, highlightLength);
      TxtResult.Focus();
    }
  }
}
