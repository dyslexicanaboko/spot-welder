using SpotWelder.Lib;
using SpotWelder.Lib.Models;
using SpotWelder.Ui.Helpers;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SpotWelder.Ui
{
  /// <summary>
  ///   Any code related to obtaining parameters will live in this partial for the sake of legibility.
  /// </summary>
  public partial class QueryToClassControl
  {
    private QueryToClassParameters? GetParameters()
    {
      var obj = CommonValidation();

      //Check if the common validation failed
      if (obj == null) return null;

      obj.LanguageType = CodeType.CSharp;
      obj.OverwriteExistingFiles = CbReplaceExistingFiles.IsCheckedAndEnabled();
      obj.Namespace = TxtNamespaceName.Text;

      if (TxtClassEntityName.IsTextInvalid("Class name cannot be empty."))
        return null;

      obj.ServerConnection.SqlEngine = SqlEngine.SqlServer;
      obj.ServerConnection.TableQuery = GetTableQueryFormatStrategy().ParseTableName(TxtSourceSqlText.Text);
      obj.SubjectName = TxtEntityName.Text;
      obj.EntityName = TxtClassEntityName.Text;
      obj.ModelName = TxtClassModelName.Text;
      obj.Elections = _electionToCheckBoxMap.GetChosenGenerationElections();

      if(obj.Elections.HasAnyFlag(
           GenerationElections.MapModelToEntity, 
           GenerationElections.MapEntityToModel, 
           GenerationElections.MapInterfaceToEntity,
           GenerationElections.MapInterfaceToModel,
           GenerationElections.MapCreateModelToEntity,
           GenerationElections.MapPatchModelToEntity))
        obj.Elections |= GenerationElections.GenerateMapper;
      
      return obj;
    }
    
    private QueryToClassParameters? CommonValidation()
    {
      var obj = new QueryToClassParameters();

      var con = ConnectionStringCb.CurrentConnection;

      if (!con.Verified && !ConnectionStringCb.TestConnectionString(true))
        return null;

      obj.ServerConnection.ConnectionString = con.ConnectionString;

      obj.ServerConnection.SourceSqlType = GetSourceType();

      if (TxtSourceSqlText.IsTextInvalid(obj.ServerConnection.SourceSqlType + " cannot be empty."))
        return null;

      obj.ServerConnection.SourceSqlText = TxtSourceSqlText.Text;
      obj.SaveAsFile = CbSaveFileOnGeneration.IsChecked();

      if (obj.SaveAsFile)
      {
        const string s = "If saving file on generation, then {0} cannot be empty.";

        if (TxtPath.IsTextInvalid(string.Format(s, "Path")))
          return null;

        if (TxtFileName.IsTextInvalid(string.Format(s, "File name")))
          return null;
      }

      obj.FilePath = TxtPath.Text;
      obj.Filename = TxtFileName.Text;

      if (_classCheckBoxGroup.HasTickedCheckBox())
        return obj;

      UserControlExtensions.ShowWarningMessage(
        "You must select at least one construct for generation. None is not an option.");

      return null;
    }
    
    private Dictionary<GenerationElections, CheckBox> GetGenerationElectionsMap()
    {
      var dict = new Dictionary<GenerationElections, CheckBox>
      {
        { GenerationElections.GenerateEntity, CbClassEntity },
        { GenerationElections.GenerateEntityIEquatable, CbClassEntityIEquatable },
        { GenerationElections.GenerateEntityIComparable, CbClassEntityIComparable },
        { GenerationElections.GenerateEntityEqualityComparer, CbClassEntityEqualityComparer },
        { GenerationElections.GenerateInterface, CbClassInterface },
        { GenerationElections.GenerateModel, CbClassModel },
        { GenerationElections.MapEntityToModel, CbMapEntityToModel },
        { GenerationElections.MapModelToEntity, CbMapModelToEntity },
        { GenerationElections.MapInterfaceToEntity, CbMapInterfaceToEntity },
        { GenerationElections.MapInterfaceToModel, CbMapInterfaceToModel },
        { GenerationElections.MapCreateModelToEntity, CbMapCreateModelToEntity },
        { GenerationElections.MapPatchModelToEntity, CbMapPatchModelToEntity },
        { GenerationElections.SerializeCsv, CbSerializeCsv },
        { GenerationElections.SerializeJson, CbSerializeJson },
        { GenerationElections.RepoStatic, CbRepoStatic },
        { GenerationElections.RepoDapper, CbRepoDapper },
        { GenerationElections.RepoEfFluentApi, CbRepoEfFluentApi },
        { GenerationElections.Service, CbService },
        { GenerationElections.ApiController, CbApiController },
        { GenerationElections.GenerateCreateModel, CbClassCreateModel },
        { GenerationElections.GeneratePatchModel, CbClassPatchModel },
        { GenerationElections.MakeAsynchronous, CbMakeAsynchronous },
      };

      return dict;
    }

    private CheckBoxGroup GetCheckBoxGroup()
    {
      var cbg = new CheckBoxGroup();
      cbg.Add(CbClassEntity);
      cbg.Add(CbClassModel);
      cbg.Add(CbClassInterface);

      return cbg;
    }

    private void BtnDynamicStatements_OnClick(object sender, RoutedEventArgs e)
    {
      //TODO: This is good for now, but might want to create a simple HTML page for this and display it as part of a web browser component
      var content =
        @"There is no point in providing dynamic generation or bulk copy options because the code is 
so generic it will not likely change for most objects. Therefore I have a separate repository 
for boiler plate starter code where I am maintaining this kind of code.

You can find it here: 
https://github.com/dyslexicanaboko/code-snippets/tree/develop/Visual%20C%23/BasicDataLayers";

      _parentResultsWindow.AddTab("Basic data layers", content);
      _parentResultsWindow.Show();
    }
  }
}
