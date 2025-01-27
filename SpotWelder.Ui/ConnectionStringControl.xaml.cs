using SpotWelder.Lib;
using SpotWelder.Lib.DataAccess;
using SpotWelder.Lib.DataAccess.SqlClients;
using SpotWelder.Lib.Models;
using SpotWelder.Ui.Profile;
using SpotWelder.Ui.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SpotWelder.Ui
{
  /// <summary>
  ///   Interaction logic for ConnectionStringControl.xaml
  /// </summary>
  public partial class ConnectionStringControl : UserControl
  {
    private IGeneralDatabaseQueries _generalRepo;
    private IConnectionStringBuilderService _builderService;

    public ConnectionStringControl()
    {
      InitializeComponent();
    }

    public ConnectionStringManager ConnectionStringManager { get; private set; }

    public UserConnectionString CurrentConnection
    {
      get
      {
        if (CbConnectionString.SelectedIndex > -1)
          return (UserConnectionString)CbConnectionString.SelectedItem;

        var obj = new UserConnectionString();
        obj.Verified = false;
        obj.ConnectionString = CbConnectionString.Text;
        obj.SqlEngine = SqlEngine.SqlServer;

        return obj;
      }
    }

    //Keeping these connection strings here just in case they get removed from the profile.json
    //"Data Source=.;Initial Catalog=ScratchSpace;Integrated Security=SSPI;"
    //"Host=localhost;Database=millions_of_things;Username=postgres;Password=postgres"
    //FYI: The index location can change
    public void DebugSetSqlServerTestParameters()
      => CbConnectionString.SelectedIndex = 6;

    public void DebugSetPostgresTestParameters()
      => CbConnectionString.SelectedIndex = 4;

    public void Dependencies(
      IProfileManager profileManager,
      IGeneralDatabaseQueries repository,
      IConnectionStringBuilderService builderService)
    {
      _generalRepo = repository;
      _builderService = builderService;

      ConnectionStringManager = profileManager.ConnectionStringManager;

      DataContext = ConnectionStringManager;
    }

    private async void BtnConnectionStringTest_Click(object sender, RoutedEventArgs e)
    {
      await TestConnectionStringNonBlocking();
    }

    private async void CbConnectionString_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
        await TestConnectionStringNonBlocking();
    }
    
    private async Task TestConnectionStringNonBlocking()
    {
      try
      {
        BtnConnectionStringTest.IsEnabled = false;
        PbConnectionTest.IsIndeterminate = true;

        var con = CurrentConnection;
        
        var result = await Task.Run(() => TestConnectionString(con));

        ShowResult(result);
      }
      finally
      {
        BtnConnectionStringTest.IsEnabled = true;
        PbConnectionTest.IsIndeterminate = false;
      }
    }

    private ConnectionResult TestConnectionString(UserConnectionString userConnectionString)
    {
      var serverConnection = new ServerConnection
      {
        SqlEngine = userConnectionString.SqlEngine,
        ConnectionString = userConnectionString.ConnectionString
      };

      if (string.IsNullOrWhiteSpace(serverConnection.ConnectionString))
      {
        return new ConnectionResult
        {
          Success = false,
          ReturnedException = new Exception("Connection string cannot be blank, empty or whitespace.")
        };
      }

      var obj = _generalRepo.TestConnectionString(serverConnection);

      userConnectionString.Verified = obj.Success;

      ConnectionStringManager.Upsert(userConnectionString);

      return obj;
    }

    private bool ShowResult(ConnectionResult result, bool showMessageOnFailureOnly = false)
    {
      var showMessage = true;

      if (showMessageOnFailureOnly)
        showMessage = !result.Success;

      if (showMessage)
        UserControlExtensions.ShowWarningMessage(
          result.Success ? "Connected Successfully" : "Connection Failed. Returned error: " + result.Message);

      return result.Success;
    }

    public bool TestConnectionString(bool showMessageOnFailureOnly = false)
    {
      var obj = TestConnectionString(CurrentConnection);

      return ShowResult(obj, showMessageOnFailureOnly);
    }

    //Edit can result in editing an existing connection or deleting it all together
    //Additionally, on the edit the existing connection information must be shown
    private void BtnEdit_OnClick(object sender, RoutedEventArgs e)
    {
      if (CbConnectionString.SelectedIndex < 0)
      {
        UserControlExtensions.ShowWarningMessage("Please select an existing connection string to perform an edit.");

        return;
      }

      LaunchConnectionStringBuilder((UserConnectionString)CbConnectionString.SelectedItem);
    }

    private void BtnAdd_OnClick(object sender, RoutedEventArgs e)
    {
      LaunchConnectionStringBuilder();
    }

    private void LaunchConnectionStringBuilder(UserConnectionString? existing = null)
    {
      var win = new ConnectionStringBuilderWindow();
      win.Dependencies(_builderService);

      if(existing != null) win.LoadConnectionString(existing);

      if (!win.ShowDialog().GetValueOrDefault()) return;

      var con = win.GetConnectionString();

      var ucs = new UserConnectionString
      {
        ConnectionString = con.ConnectionString,
        SqlEngine = con.SqlEngine,
        Verified = true //Hard coding to true for now
      };

      if (con.Operation == Enumerations.Upsert)
      {
        ConnectionStringManager.Upsert(ucs);
      }
      else
      {
        ConnectionStringManager.Remove(ucs);
      }

      CbConnectionString.SelectedIndex = 0;
    }

    private void CbConnectionString_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var sqlEngine = e.AddedItems.Count == 0 ? 
        null : 
        (SqlEngine?)((UserConnectionString)e.AddedItems[0]).SqlEngine;

      ImgLogo.Source = ImageSelectionHelper.GetConnectionStringLogo(sqlEngine);
    }
  }
}
