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

      obj.TableQuery = _svcNameFormat.ParseTableName(TxtSourceSqlText.Text);
      obj.SubjectName = TxtEntityName.Text;
      obj.EntityName = TxtClassEntityName.Text;
      obj.ModelName = TxtClassModelName.Text;
      obj.Elections = GetChosenGenerationElections();

      //TODO: Hook this up to the UI
      //This is temporarily hardcoded to true for testing
      obj.ApiRoute = "tasks";

      return obj;
    }

    private GenerationElections GetChosenGenerationElections()
    {
      var e = GenerationElections.None;

      foreach (var kvp in _electionToCheckBoxMap)
      {
        if (!kvp.Value.IsChecked()) continue;

        e |= kvp.Key;
      }

      //TODO: Hook this up to the UI
      //This is temporarily hardcoded to true for testing
      e |= GenerationElections.Service;
      e |= GenerationElections.ApiController;
      e |= GenerationElections.GenerateCreateModel;
      e |= GenerationElections.GeneratePatchModel;
      e |= GenerationElections.MakeAsynchronous;

      return e;
    }

    private QueryToClassParameters? CommonValidation()
    {
      var obj = new QueryToClassParameters();

      var con = ConnectionStringCb.CurrentConnection;

      if (!con.Verified && !ConnectionStringCb.TestConnectionString(true))
        return null;

      obj.ConnectionString = con.ConnectionString;

      obj.SourceSqlType = GetSourceType();

      if (TxtSourceSqlText.IsTextInvalid(obj.SourceSqlType + " cannot be empty."))
        return null;

      obj.SourceSqlText = TxtSourceSqlText.Text;
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
        { GenerationElections.CloneEntityToModel, CbCloneEntityToModel },
        { GenerationElections.CloneModelToEntity, CbCloneModelToEntity },
        { GenerationElections.CloneInterfaceToEntity, CbCloneInterfaceToEntity },
        { GenerationElections.CloneInterfaceToModel, CbCloneInterfaceToModel },
        { GenerationElections.SerializeCsv, CbSerializeCsv },
        { GenerationElections.SerializeJson, CbSerializeJson },
        { GenerationElections.RepoStatic, CbRepoStatic },
        { GenerationElections.RepoDapper, CbRepoDapper },
        { GenerationElections.RepoEfFluentApi, CbRepoEfFluentApi }
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
