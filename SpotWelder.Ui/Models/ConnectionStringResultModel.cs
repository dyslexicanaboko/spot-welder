using SpotWelder.Lib;
using SpotWelder.Ui.ViewModels;

namespace SpotWelder.Ui.Models
{
  public class ConnectionStringResultModel
  {
    public ConnectionStringResultModel(ConnectionStringViewModel viewModel)
    {
      Operation = viewModel.Operation;
      ConnectionString = viewModel.ConnectionString;
      SqlEngine = viewModel.SqlEngine.Value;
    }

    public Enumerations Operation { get; set; }

    public string ConnectionString { get; set; }

    public SqlEngine SqlEngine { get; set; }
  }
}
