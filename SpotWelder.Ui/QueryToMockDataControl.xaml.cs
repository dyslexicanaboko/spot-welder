using SpotWelder.Lib;
using SpotWelder.Lib.DataAccess;
using SpotWelder.Lib.DataAccess.SqlClients;
using SpotWelder.Lib.Events;
using SpotWelder.Lib.Exceptions;
using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services;
using SpotWelder.Lib.Services.TableQueryFormats;
using SpotWelder.Ui.Helpers;
using SpotWelder.Ui.Profile;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using B = SpotWelder.Ui.UserControlExtensions;

namespace SpotWelder.Ui
{
  /// <summary>
  ///   Interaction logic for QueryToMockDataControl.xaml
  /// </summary>
  public partial class QueryToMockDataControl : UserControl, IUsesResultWindow
  {
    private readonly ResultWindowManager _resultWindowManager;

    private IGeneralDatabaseQueries _repoGeneral;

    private IQueryToMockDataService _svcQueryToMockData;

    private ITableQueryFormatFactory _tableQueryFormatFactory;

    // Empty constructor Required by WPF
    public QueryToMockDataControl()
    {
      InitializeComponent();

      _resultWindowManager = new ResultWindowManager();

      TxtClassEntityName.DefaultButton_UnregisterDefaultEvent();
      TxtClassEntityName.DefaultButton.Click += BtnClassEntityNameDefault_Click;
    }

    public void CloseResultWindows() => _resultWindowManager.CloseAll();

    public void Dependencies(
      ITableQueryFormatFactory tableQueryFormatFactory,
      IQueryToMockDataService queryToMockDataService,
      IGeneralDatabaseQueries repository,
      IProfileManager profileManager, 
      IConnectionStringBuilderService builderService)
    {
      _tableQueryFormatFactory = tableQueryFormatFactory;
      _repoGeneral = repository;

      _svcQueryToMockData = queryToMockDataService;

      ConnectionStringCb.Dependencies(profileManager, _repoGeneral, builderService);
    }

    private void TxtSqlSourceText_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
      if (RbSourceTypeTableName.IsChecked != true) return;

      try
      {
        FormatTableName(TxtSourceSqlText);

        TxtClassEntityName.Text = GetDefaultClassName();
      }
      catch (Exception ex)
      {
        UserControlExtensions.ShowWarningMessage(
          $"The table name you provided could not be formatted.\nPlease select the Query radio button if your source is not just a table name.\n\nError: {ex.Message}");
      }
    }

    private void BtnPreview_OnClick(object sender, RoutedEventArgs e)
    {
      try
      {
        var p = GetParameters();

        if (p == null) return;

        TbEntityResult.Text = _svcQueryToMockData.GetEntity(p);

        TbMockDataResults.Text = _svcQueryToMockData.GetMockData(p, 5).Contents;

        LblPreviewTimestamp.Content = DateTime.Now.ToString(CultureInfo.CurrentCulture);
      }
      catch (Exception ex)
      {
        UserControlExtensions.ShowErrorMessage(ex);
      }
    }

    private ITableQueryFormatStrategy GetTableQueryFormatStrategy()
      => _tableQueryFormatFactory.GetStrategy(ConnectionStringCb.CurrentConnection.SqlEngine);

    private void FormatTableName(TextBox target)
    {
      var strName = target.Text;

      if (string.IsNullOrWhiteSpace(strName))
        return;

      target.Text = GetTableQueryFormatStrategy().FormatTableQuery(strName);
    }

    private QueryToMockDataParameters? GetParameters()
    {
      var obj = CommonValidation();

      //Check if the common validation failed
      if (obj == null) return null;

      if (TxtClassEntityName.IsTextInvalid("Class name cannot be empty."))
        return null;

      obj.ClassEntityName = TxtClassEntityName.Text;

      obj.ServerConnection.TableQuery = GetTableQueryFormatStrategy().ParseTableName(TxtSourceSqlText.Text);

      return obj;
    }

    private QueryToMockDataParameters? CommonValidation()
    {
      var obj = new QueryToMockDataParameters();

      var con = ConnectionStringCb.CurrentConnection;

      if (!con.Verified && !ConnectionStringCb.TestConnectionString(true))
        return null;

      obj.ServerConnection.ConnectionString = con.ConnectionString;

      obj.ServerConnection.SourceSqlType = GetSourceType();

      if (TxtSourceSqlText.IsTextInvalid(obj.ServerConnection.SourceSqlType + " cannot be empty."))
        return null;

      obj.ServerConnection.SourceSqlText = TxtSourceSqlText.Text;

      return obj;
    }

    private void BtnClassEntityNameDefault_Click(object sender, RoutedEventArgs e)
    {
      TxtClassEntityName.Text = GetDefaultClassName();
    }

    private string GetDefaultClassName()
    {
      string strName;

      try
      {
        if (RbSourceTypeTableName.IsChecked == true)
        {
          var strategy = GetTableQueryFormatStrategy();

          var tbl = strategy.ParseTableName(TxtSourceSqlText.Text);

          strName = strategy.GetClassName(tbl);
        }
        else
        {
          strName = "Class1";
        }
      }
      catch
      {
        strName = "Class1";
      }

      return strName;
    }

    private async void BtnGenerate_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        _svcQueryToMockData.RowProcessed += MockData_RowProcessed;

        var obj = GetParameters();

        if (obj == null) return;

        PbGenerator.Value = 0;

        var result = await Task.Run(() => _svcQueryToMockData.GetMockData(obj));

        ShowResultWindow(result.Filename, result.Contents);
      }
      catch (NonUniqueColumnException nucEx)
      {
        UserControlExtensions.ShowWarningMessage(nucEx.Message);
      }
      catch (Exception ex)
      {
        UserControlExtensions.ShowErrorMessage(ex);
      }
      finally
      {
        _svcQueryToMockData.RowProcessed -= MockData_RowProcessed;
      }
    }

    private void MockData_RowProcessed(object sender, RowProcessedEventArgs e)
    {
      PbGenerator.Dispatcher.BeginInvoke(
        DispatcherPriority.Normal,
        new DispatcherOperationCallback(
          delegate
          {
            PbGenerator.Maximum = e.Total;
            PbGenerator.Value = e.Count;

            var percent = e.Count / (double)e.Total;

            LblPbValues.Content = $"{e.Count}/{e.Total} = {percent:P}";

            return null;
          }),
        null);
    }

    private void ShowResultWindow(string title, string contents)
    {
      var win = new ResultWindow(title, contents);

      win.Show();

      _resultWindowManager.Add(win);
    }

    private SourceSqlType GetSourceType() => RbSourceTypeQuery.IsChecked.GetValueOrDefault() ?
      SourceSqlType.Query :
      SourceSqlType.TableName;
  }
}
