using SpotWelder.Lib;
using SpotWelder.Lib.DataAccess.SqlClients;
using SpotWelder.Ui.Models;
using SpotWelder.Ui.Profile;
using SpotWelder.Ui.Services;
using SpotWelder.Ui.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SpotWelder.Ui
{
  /// <summary>
  ///   Interaction logic for ConnectionStringBuilder.xaml
  /// </summary>
  public partial class ConnectionStringBuilderWindow : Window
  {
    //public readonly SqlEngine[] SqlEngines;
    private readonly ConnectionStringViewModel _viewModel = new();
    private IConnectionStringBuilderService _builderService;

    public ConnectionStringBuilderWindow()
    {
      InitializeComponent();

      _viewModel.SqlEngines = Enum.GetValues<SqlEngine>()
        .Select(x => new SqlEngineViewModel(x))
        .ToArray();

      DataContext = _viewModel;
    }

    public void Dependencies(IConnectionStringBuilderService builderService)
    {
      _builderService = builderService;
    }

    public ConnectionStringResultModel GetConnectionString() => new(_viewModel);

    private void CbSqlEngine_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      _viewModel.SqlEngine = (SqlEngineViewModel)CbSqlEngine.SelectedItem;

      ImgSqlEngine.Source = ImageSelectionHelper.GetConnectionStringLogo(_viewModel.SqlEngine.Value);

      if (_viewModel.SqlEngine.Value == SqlEngine.SqlServer)
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

      UpdateConnectionString();
    }

    private void TxtField_OnTextChanged(object sender, TextChangedEventArgs e)
    {
      UpdateConnectionString();
    }

    private void CheckBox_OnToggle(object sender, RoutedEventArgs e)
    {
      UpdateConnectionString();
    }
    
    private void UpdateConnectionString()
    {
      _viewModel.ConnectionString = _builderService.Build(_viewModel, _viewModel.SqlEngine.Value);

      TxtPreview.Text = _viewModel.ConnectionString;
    }

    //Serves both Add and Edit
    private void BtnSaveChanges_OnClick(object sender, RoutedEventArgs e)
    {
      UpdateConnectionString();

      DialogResult = true;

      _viewModel.Operation = Enumerations.Upsert;
    }

    private void BtnDelete_OnClick(object sender, RoutedEventArgs e)
    {
      DialogResult = true;

      _viewModel.Operation = Enumerations.Remove;
    }

    public void LoadConnectionString(UserConnectionString existing)
    {
      var meta = _builderService.Parse(existing.ConnectionString, existing.SqlEngine);

      _viewModel.SqlEngine = new SqlEngineViewModel(existing.SqlEngine);
      _viewModel.ServerName  = meta.ServerName;
      _viewModel.DatabaseName = meta.DatabaseName;
      _viewModel.Username = meta.Username;
      _viewModel.Password = meta.Password;
      _viewModel.IsEncrypted = meta.IsEncrypted;
      _viewModel.IsIntegratedSecurity = meta.IsIntegratedSecurity;

      CbSqlEngine.SelectedValue = existing.SqlEngine;

      UpdateConnectionString();
    }
  }
}
