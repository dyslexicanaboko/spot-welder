using SpotWelder.Lib;

namespace SpotWelder.Ui.ViewModels
{
  public class SqlEngineViewModel 
  {
    public SqlEngineViewModel(SqlEngine sqlEngine)
    {
      Text = sqlEngine.ToString();
      Value = sqlEngine;
    }

    public string Text { get; set; }

    public SqlEngine Value { get; set; }
  }
}
