using SpotWelder.Lib;
using SpotWelder.Lib.DataAccess;
using SpotWelder.Lib.Exceptions;
using SpotWelder.Lib.Services;
using SpotWelder.Ui.Helpers;
using SpotWelder.Ui.Profile;
using System;
using System.Collections.Generic;
using System.IO;
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

    private readonly ParentResultsWindow _parentResultsWindow;

    private readonly Dictionary<GenerationElections, CheckBox> _electionToCheckBoxMap;

    private IGeneralDatabaseQueries _generalRepo;

    private INameFormatService _svcNameFormat;

    private IQueryToClassService _svcQueryToClass;

    // Empty constructor Required by WPF
    public QueryToClassControl()
    {
      InitializeComponent();

      SetPathAsDefault();
      
      _parentResultsWindow = new ParentResultsWindow();

      TxtNamespaceName.ApplyDefault();

      TxtEntityName.DefaultButton_UnregisterDefaultEvent();
      TxtEntityName.DefaultButton.Click += BtnEntityNameDefault_Click;

      TxtClassEntityName.TextBox.TextChanged += TxtClassEntityName_TextChanged;
      TxtClassEntityName.TextBox.MouseDown += TxtClassEntityName_MouseDown;
      TxtClassEntityName.DefaultButton_UnregisterDefaultEvent();
      TxtClassEntityName.DefaultButton.Click += BtnClassEntityNameDefault_Click;

      TxtClassModelName.DefaultButton_UnregisterDefaultEvent();
      TxtClassModelName.DefaultButton.Click += (_, _) => SetModelName();

      _electionToCheckBoxMap = GetGenerationElectionsMap();
      _classCheckBoxGroup = GetCheckBoxGroup();

      DebugSetTestParameters();
    }

    private void DebugSetTestParameters()
    {
      #if DEBUG
      ConnectionStringCb.DebugSetTestParameters();

      RbSourceTypeTableName.IsChecked = true;
      RbSourceTypeQuery.IsChecked = false;

      TxtSourceSqlText.Text = "dbo.Task";
      TxtNamespaceName.Text = "Namespace1";
      TxtEntityName.Text = "Task";
      TxtClassEntityName.Text = "TaskEntity";
      TxtClassModelName.Text = "TaskModel";
      
      CbRepoDapper.IsChecked = true;

      //Entity
      CbClassEntity.IsChecked = true;
      CbClassEntityIEquatable.IsChecked = true;
      CbClassEntityIComparable.IsChecked = true;

      //Interface
      CbClassInterface.IsChecked = true;

      //Models
      CbClassModel.IsChecked = true;
      CbClassCreateModel.IsChecked = true;
      CbClassPatchModel.IsChecked = true;

      //Services
      CbClassEntityEqualityComparer.IsChecked = true;
      CbSerializeCsv.IsChecked = true;
      CbSerializeJson.IsChecked = true;

      //Layers
      CbMakeAsynchronous.IsChecked = true;
      CbApiController.IsChecked = true;
      CbService.IsChecked = true;

      //Mappings
      CbMapInterfaceToModel.IsChecked = true;
      CbMapInterfaceToEntity.IsChecked = true;
      CbMapEntityToModel.IsChecked = true;
      CbMapModelToEntity.IsChecked = true;
      CbMapCreateModelToEntity.IsChecked = true;
      CbMapPatchModelToEntity.IsChecked = true;
      #endif
    }
    
    private static string DefaultPath => AppDomain.CurrentDomain.BaseDirectory;

    public void CloseResultWindows() => _parentResultsWindow.Shutdown();

    public void Dependencies(
      INameFormatService nameFormatService,
      IQueryToClassService queryToClassService,
      IGeneralDatabaseQueries repository,
      IProfileManager profileManager)
    {
      _svcNameFormat = nameFormatService;
      _svcQueryToClass = queryToClassService;
      _generalRepo = repository;

      ConnectionStringCb.Dependencies(profileManager, _generalRepo);
    }

    private void TxtSqlSourceText_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
      if (RbSourceTypeTableName.IsChecked != true) return;

      try
      {
        FormatTableName(TxtSourceSqlText);

        TxtEntityName.Text = GetDefaultEntityName();

        TxtClassEntityName.Text = GetDefaultClassName();
      }
      catch (Exception ex)
      {
        UserControlExtensions.ShowWarningMessage(
          $"The table name you provided could not be formatted.\nPlease select the Query radio button if your source is not just a table name.\n\nError: {ex.Message}");
      }
    }

    private void FormatTableName(TextBox target)
    {
      var strName = target.Text;

      if (string.IsNullOrWhiteSpace(strName))
        return;

      target.Text = _svcNameFormat.FormatTableQuery(strName);
    }

    private void BtnClassEntityNameDefault_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        TxtClassEntityName.Text = GetDefaultClassName();

        if (string.IsNullOrWhiteSpace(TxtFileName.Text))
          TxtFileName.Text = TxtClassEntityName.Text + ".cs";
      }
      catch
      {
        TxtClassEntityName.Text = "Class1";
      }
    }

    private void BtnEntityNameDefault_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        TxtEntityName.Text = GetDefaultEntityName();
      }
      catch
      {
        TxtEntityName.Text = "Entity1";
      }
    }

    private string GetDefaultEntityName()
    {
      var tbl = _svcNameFormat.ParseTableName(TxtSourceSqlText.Text);

      var entity = _svcNameFormat.GetClassName(tbl);

      return entity;
    }

    private string GetDefaultClassName(bool includeExtension = false)
    {
      string strName;

      if (RbSourceTypeTableName.IsChecked == true)
      {
        strName = GetDefaultEntityName();
      }
      else
      {
        var i = 0;

        var strDir = GetPath();
        var strExt = includeExtension ? ".cs" : string.Empty;

        strName = "Class_" + i + strExt; //Must prime the string

        while (FileExist(strDir, strName))
        {
          i++;

          strName = "Class_" + i + strExt;
        }
      }

      if (string.IsNullOrWhiteSpace(strName))
        strName = TxtClassEntityName.DefaultText;
      else
        strName += "Entity";

      return strName;
    }

    private bool FileExist(string path, string fileName) => File.Exists(Path.Combine(path, fileName));

    private string GetPath()
    {
      var strPath = TxtPath.Text;

      if (!Directory.Exists(strPath))
        strPath = SetPathAsDefault();

      return strPath;
    }

    private string SetPathAsDefault()
    {
      TxtPath.Text = DefaultPath;

      return TxtPath.Text;
    }

    private void CbSaveFileOnGeneration_Checked(object sender, RoutedEventArgs e)
    {
      ToggleSaveFileOnGenerationDependentControls(CbSaveFileOnGeneration.IsChecked());
    }

    private void ToggleSaveFileOnGenerationDependentControls(bool isEnabled)
    {
      CbReplaceExistingFiles.IsEnabled = isEnabled;

      BtnPathDefault.IsEnabled = isEnabled;
      BtnFileNameDefault.IsEnabled = isEnabled;

      TxtFileName.IsEnabled = isEnabled;
      TxtPath.IsEnabled = isEnabled;

      LblFileName.IsEnabled = isEnabled;
      LblPath.IsEnabled = isEnabled;
    }

    private void BtnPathDefault_Click(object sender, RoutedEventArgs e)
    {
      SetPathAsDefault();
    }

    private void BtnFileNameDefault_Click(object sender, RoutedEventArgs e)
    {
      if (string.IsNullOrWhiteSpace(TxtPath.Text))
        SetPathAsDefault();

      TxtFileName.Text = GetDefaultClassName(true);
    }

    private void TxtClassEntityName_TextChanged(object sender, TextChangedEventArgs e)
    {
      SetFileName();

      if (!string.IsNullOrWhiteSpace(TxtClassModelName.Text)) return;

      SetModelName();
    }

    private void SetModelName()
    {
      var entity = TxtClassEntityName.Text;

      //If it ends with "Entity" get the index
      var i = entity.LastIndexOf("Entity", StringComparison.InvariantCultureIgnoreCase);

      //If it is found, remove it
      if (i > 0)
        entity = entity.Remove(i);

      TxtClassModelName.Text = entity + "Model";
    }

    private void SetFileName()
    {
      //On startup this control is null and so it throws an exception
      if (TxtFileName == null) return;

      TxtFileName.Text = TxtClassEntityName.Text + ".cs";
    }

    private void TxtFileName_TextChanged(object sender, TextChangedEventArgs e)
    {
      try
      {
        if (FileExist(GetPath(), TxtFileName.Text))
          TxtFileName.Text = GetDefaultClassName(true) + ".cs";
      }
      catch
      {
        //Trap Exception
      }
    }

    private async void BtnGenerate_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        var obj = GetParameters();

        if (obj == null) return;

        PbGenerator.IsIndeterminate = true;

        var results = await Task.Run(() => _svcQueryToClass.Generate(obj));

        foreach (var g in results) _parentResultsWindow.AddTab(g.Filename, g.Contents);

        _parentResultsWindow.Show();
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
        PbGenerator.IsIndeterminate = false;
      }
    }

    private SourceSqlType GetSourceType() => RbSourceTypeQuery.IsChecked.GetValueOrDefault() ?
      SourceSqlType.Query :
      SourceSqlType.TableName;

    private void TxtClassEntityName_MouseDown(object sender, MouseButtonEventArgs e)
    {
      TxtClassEntityName.TextBox.SelectAll();
    }

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
      CbMapInterfaceToEntity.IsEnabled = isChecked;

      CbClassModelAndEntity_ToggleJointDependents(isChecked, CbClassModel.IsChecked());
    }

    private void CbClassModel_ToggleDependents()
    {
      if (CbMapEntityToModel == null) return; //On Startup controls are still null

      var isChecked = CbClassModel.IsChecked();

      CbMapInterfaceToModel.IsEnabled = isChecked;

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
