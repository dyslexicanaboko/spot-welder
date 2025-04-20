using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SpotWelder.Ui.Controls
{
  /// <summary>
  /// Interaction logic for TextResultControl.xaml
  /// </summary>
  public partial class TextResultControl : UserControl
  {
    public TextResultControl()
    {
      InitializeComponent();
    }
    
    public static readonly DependencyProperty TextProperty =
      DependencyProperty.Register(
        nameof(Text), 
        typeof(string), 
        typeof(TextResultControl),
        new FrameworkPropertyMetadata(default(string)));

    public string Text 
    {
      get => (string)GetValue(TextProperty);
      set => SetValue(TextProperty, value);
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

    private void TxtResult_OnTargetUpdated(object? sender, DataTransferEventArgs e)
    {
      if (sender is not TextBox txt) return;

      LineNumbers.Text = GetLineNumbers(txt.Text);
    }

    private void CommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      if (e.Command != ApplicationCommands.Copy) return;

      Clipboard.SetText(TxtResult.Text);
    }
  }
}
