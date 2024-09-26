using Newtonsoft.Json.Linq;
using SpotWelder.Lib;
using SpotWelder.Ui.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SpotWelder.Ui
{
  /// <summary>
  /// Interaction logic for ConnectionStringBuilder.xaml
  /// </summary>
  public partial class ConnectionStringBuilder : Window
  {
    //public readonly SqlEngine[] SqlEngines;
    private readonly ConnectionStringViewModel _viewModel = new();

    public ConnectionStringBuilder()
    {
      InitializeComponent();

      //SqlEngines = Enum.GetValues<SqlEngine>();

      DataContext = _viewModel;
    }
    
    public (SqlEngine, string) GetConnectionString() => (_viewModel.SqlEngine, _viewModel.ConnectionString);

    private void CbSqlEngine_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if(_viewModel.SqlEngine == SqlEngine.SqlServer)
      {
        LblIsEncrypted.Visibility = Visibility.Visible;
        LblIntegratedSecurity.Visibility = Visibility.Visible;
        CbIsEncrypted.Visibility = Visibility.Visible;
        CbIntegratedSecurity.Visibility = Visibility.Visible;
      }
      else
      {
        LblIsEncrypted.Visibility = Visibility.Collapsed;
        LblIntegratedSecurity.Visibility = Visibility.Collapsed;
        CbIsEncrypted.Visibility = Visibility.Collapsed;
        CbIntegratedSecurity.Visibility = Visibility.Collapsed;
      }

      _viewModel.ConnectionString = BuildConnectionString(_viewModel);
    }

    private void TxtField_OnTextChanged(object sender, TextChangedEventArgs e)
    {
      _viewModel.ConnectionString = BuildConnectionString(_viewModel);
    }

    private void CheckBox_OnToggle(object sender, RoutedEventArgs e)
    {
      _viewModel.ConnectionString = BuildConnectionString(_viewModel);
    }

    //This won't be used anywhere else, so I don't think it needs to be centralized
    //I might change my mind later. It doesn't really belong here, but it also won't be reused.
    private static string BuildConnectionString(ConnectionStringViewModel viewModel)
    {
      if (viewModel.SqlEngine == SqlEngine.SqlServer)
      {
        var sbS = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder();
        sbS.DataSource = viewModel.ServerName;
        sbS.InitialCatalog = viewModel.DatabaseName;
        sbS.UserID = viewModel.Username;
        sbS.Password = viewModel.Password;
        sbS.IntegratedSecurity = viewModel.IsIntegratedSecurity;
        sbS.Encrypt = viewModel.IsEncrypted;

        return sbS.ToString();
      }

      var sbP = new Npgsql.NpgsqlConnectionStringBuilder();
      sbP.Host = viewModel.ServerName;
      sbP.Database = viewModel.DatabaseName;
      sbP.Username = viewModel.Username;
      sbP.Password = viewModel.Password;

      return sbP.ToString();
    }
  }
}
