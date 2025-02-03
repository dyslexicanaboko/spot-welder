using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;

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
  }
}
