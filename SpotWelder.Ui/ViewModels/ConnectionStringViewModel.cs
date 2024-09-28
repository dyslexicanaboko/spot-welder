using SpotWelder.Lib.Models;

namespace SpotWelder.Ui.ViewModels
{
  public class ConnectionStringViewModel
    : ConnectionStringMeta
  {
    public SqlEngineViewModel SqlEngine { get; set; } = new(Lib.SqlEngine.SqlServer);
    
    public string ConnectionString { get; set; } = string.Empty;

    public SqlEngineViewModel[] SqlEngines { get; set; } = { };

    public Enumerations Operation { get; set; }
  }
}
