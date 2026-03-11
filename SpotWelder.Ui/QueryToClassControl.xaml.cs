using Microsoft.Extensions.Logging;
using SpotWelder.Lib;
using SpotWelder.Lib.Exceptions;
using SpotWelder.Lib.Services;
using SpotWelder.Lib.Services.TableQueryFormats;
using SpotWelder.Ui.Controls;
using SpotWelder.Ui.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SpotWelder.Ui
{
  /// <summary>
  ///   Interaction logic for QueryToClassControl.xaml
  /// </summary>
  public partial class QueryToClassControl : UserControl, IUsesResultWindow
  {
    private readonly CheckBoxGroup _classCheckBoxGroup;

    private readonly Dictionary<GenerationElections, CheckBox> _electionToCheckBoxMap;

    private readonly ParentResultsWindow _parentResultsWindow;

    private IQueryToClassService _svcQueryToClass;

    private ITableQueryFormatFactory _tableQueryFormatFactory;

    private ILogger<QueryToClassControl> _logger;

    // Empty constructor Required by WPF
    public QueryToClassControl()
    {
      InitializeComponent();

      _parentResultsWindow = new ParentResultsWindow();

      TxtNamespaceName.ApplyDefault();

      TxtSubjectName.DefaultButton_UnregisterDefaultEvent();
      TxtSubjectName.DefaultButton.Click += BtnSubjectNameDefault_Click;

      _electionToCheckBoxMap = GetGenerationElectionsMap();
      _classCheckBoxGroup = GetCheckBoxGroup();

      Loaded += QueryToClassControl_Loaded;
    }

    private void QueryToClassControl_Loaded(object sender, RoutedEventArgs e)
    {
#if DEBUG
      //These methods have been moved to a partial class
      DebugOneTableSqlServerTest();
      //DebugCompoundQuerySqlServerTest();
      //DebugWholeSqlServerTest();
      //DebugMinimalPostgresTest();
      //DebugWholeSqlServerTestForParity();
      //DebugWholePostgresTestForParity();
#endif
      Loaded -= QueryToClassControl_Loaded;
    }

    public void CloseResultWindows() => _parentResultsWindow.Shutdown();

    private ITableQueryFormatStrategy GetTableQueryFormatStrategy()
      => _tableQueryFormatFactory.GetStrategy(ConnectionStringCb.CurrentConnection.SqlEngine);

    public void Dependencies(QueryToClassControlDependencies dependencies)
    {
      _logger = dependencies.Logger;
      _tableQueryFormatFactory = dependencies.TableQueryFormatFactory;
      _svcQueryToClass = dependencies.QueryToClassService;

      ConnectionStringCb.Dependencies(dependencies.ConnectionStringControlDependencies);

      _parentResultsWindow.ErrorOccurred += (_, childArgs) =>
      {
        _logger.LogError(childArgs.Exception, nameof(_parentResultsWindow) + " " + childArgs.Message);
      };
    }

    private void TxtSqlSourceText_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
      if (RbSourceTypeTableName.IsChecked != true) return;

      try
      {
        FormatTableName(TxtSourceSqlText);

        TxtSubjectName.Text = GetDefaultEntityName();
      }
      catch (Exception ex)
      {
        UserControlExtensions.ShowWarningMessage(
          $"The table name you provided could not be formatted.\nPlease select the Query radio button if your source is not just a table name.\n\nError: {ex.Message}");

        _logger.LogError(ex, "Table name could not be formatted.");
      }
    }

    private void FormatTableName(TextBox target)
    {
      var strName = target.Text;

      if (string.IsNullOrWhiteSpace(strName))
        return;

      target.Text = GetTableQueryFormatStrategy().FormatTableQuery(strName);
    }

    private void BtnSubjectNameDefault_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        TxtSubjectName.Text = GetDefaultEntityName();
      }
      catch
      {
        TxtSubjectName.Text = "Entity1";
      }
    }

    private string GetDefaultEntityName()
    {
      var strategy = GetTableQueryFormatStrategy();

      var tbl = strategy.ParseTableName(TxtSourceSqlText.Text);

      return strategy.GetClassName(tbl); //Entity
    }
    
    private async void BtnGenerate_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        var parameters = GetParameters();

        if (parameters == null) return;

        if (!parameters.HasElections)
        {
          UserControlExtensions.ShowWarningMessage("No elections were made. Make elections to continue.");

          return;
        }

        PbGenerator.IsIndeterminate = true;

        var results = await Task.Run(() => _svcQueryToClass.Generate(parameters));

        if (results == null)
        {
          #if DEBUG
          //You cannot do multiple assignments on the same row. Do one per row. `e.Elections |= election`
          UserControlExtensions.ShowWarningMessage(
            $"Results was null. Elections equals {(int)parameters.Elections}. Did you modify the flags of the GenerationElections enum?");
          #endif

          UserControlExtensions.ShowWarningMessage(
            "No results were returned. This is not the expected behavior (bug?).");

          return;
        }
        
        //NOTE: The results are aggregated into a single file list in the generator
        foreach (var r in results) 
          _parentResultsWindow.AddTab(r.Filename, r.Contents, r.ContainingNamespace);

        //Cannot use Show() here directly because of how this window is being invoked.
        //In order to show this window where the parent window is, it has to be shown first.
        _parentResultsWindow.ShowOnActiveWindow();
      }
      catch (NonUniqueColumnException nucEx)
      {
        UserControlExtensions.ShowWarningMessage(nucEx.Message);
      }
      catch (Exception ex)
      {
        UserControlExtensions.ShowErrorMessage(ex);

        _logger.LogError(ex, "Error during Query to class generation");
      }
      finally
      {
        PbGenerator.IsIndeterminate = false;
      }
    }

    private SourceSqlType GetSourceType() => RbSourceTypeQuery.IsChecked.GetValueOrDefault() ?
      SourceSqlType.Query :
      SourceSqlType.TableName;

    private ArchitectureType GetArchitectureType() => RbArchitectureNTier.IsChecked.GetValueOrDefault() ?
      ArchitectureType.NTier :
      ArchitectureType.FeatureBased;

    private void CbClassEntity_OnChecked(object sender, RoutedEventArgs e)
      => CbClassEntity_ToggleDependents();

    private void CbClassEntity_OnUnchecked(object sender, RoutedEventArgs e)
      => CbClassEntity_ToggleDependents();

    private void CbClassModel_Checked(object sender, RoutedEventArgs e)
      => CbClassModel_ToggleDependents();

    private void CbClassModel_Unchecked(object sender, RoutedEventArgs e)
      => CbClassModel_ToggleDependents();

    private void CbClassEntity_ToggleDependents()
    {
      if (CbClassEntityEqualityComparer == null) return; //On Startup controls are still null

      var isChecked = CbClassEntity.IsChecked();

      CbClassEntityEqualityComparer.IsEnabled = isChecked;
      CbClassEntityIEquatable.IsEnabled = isChecked;
      CbClassEntityIComparable.IsEnabled = isChecked;

      CbClassModelAndEntity_ToggleJointDependents(isChecked, CbClassModel.IsChecked());
    }

    private void CbClassModel_ToggleDependents()
    {
      if (CbMapEntityToModel == null) return; //On Startup controls are still null

      var isChecked = CbClassModel.IsChecked();

      CbClassModelAndEntity_ToggleJointDependents(CbClassEntity.IsChecked(), isChecked);
    }

    private void CbClassModelAndEntity_ToggleJointDependents(bool isEntityChecked, bool isModelChecked)
    {
      var both = isEntityChecked && isModelChecked;

      CbMapEntityToModel.IsEnabled = both;
      CbMapModelToEntity.IsEnabled = both;
    }
  }
}
