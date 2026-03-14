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
      obj.RootContainingNamespace = TxtRootContainingNamespace.Text;

      if (TxtSubjectName.IsTextInvalid("Class name cannot be empty."))
        return null;

      obj.SubjectName = TxtSubjectName.Text;
      obj.Elections = _electionToCheckBoxMap.GetChosenGenerationElections();

      //Election modifications has been moved to the service class
      //Go look at the Generate() method.
      
      return obj;
    }
    
    private QueryToClassParameters? CommonValidation()
    {
      var obj = new QueryToClassParameters();

      var con = ConnectionStringCb.CurrentConnection;

      if (!con.Verified && !ConnectionStringCb.TestConnectionString(true))
        return null;

      obj.ServerConnection.ConnectionString = con.ConnectionString;
      obj.ServerConnection.SqlEngine = con.SqlEngine;
      obj.ServerConnection.SourceSqlType = GetSourceType();

      if (TxtSourceSqlText.IsTextInvalid(obj.ServerConnection.SourceSqlType + " cannot be empty."))
        return null;
      
      var strategy = GetTableQueryFormatStrategy();

      obj.ServerConnection.SourceSqlText = TxtSourceSqlText.Text;
      obj.ServerConnection.TableQuery = strategy.ParseTableName(TxtSourceSqlText.Text);

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
        { GenerationElections.Entity, CbClassEntity },
        { GenerationElections.EntityIEquatable, CbClassEntityIEquatable },
        { GenerationElections.EntityIComparable, CbClassEntityIComparable },
        { GenerationElections.EntityEqualityComparer, CbClassEntityEqualityComparer },
        { GenerationElections.Interface, CbClassInterface },
        { GenerationElections.Model, CbClassModel },
        { GenerationElections.MapEntityToModel, CbMapEntityToModel },
        { GenerationElections.MapModelToEntity, CbMapModelToEntity },
        { GenerationElections.MapCreateModelToEntity, CbMapCreateModelToEntity },
        { GenerationElections.MapPatchModelToEntity, CbMapPatchModelToEntity },
        { GenerationElections.MapEntityToCreatedModel, CbMapEntityToCreatedModel },
        { GenerationElections.SerializeCsv, CbSerializeCsv },
        { GenerationElections.SerializeJson, CbSerializeJson },
        { GenerationElections.RepoStatic, CbRepoStatic },
        { GenerationElections.RepoDapper, CbRepoDapper },
        { GenerationElections.Manager, CbManager },
        { GenerationElections.ApiController, CbApiController },
        { GenerationElections.CreateModel, CbClassCreateModel },
        { GenerationElections.PatchModel, CbClassPatchModel },
        { GenerationElections.CreatedModel, CbClassCreatedModel },
        { GenerationElections.MakeAsynchronous, CbMakeAsynchronous },
        { GenerationElections.Immutables, CbImmutables },
        { GenerationElections.Validation, CbValidation },
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

    //TODO: This will also be part of the generated documentation in Mark Down.
    private void BtnDynamicStatements_OnClick(object sender, RoutedEventArgs e)
    {
      const string content = 
        """
        There is no point in providing dynamic generation or bulk copy options because the code is 
        so generic it will not likely change for most objects. Therefore I have a separate repository 
        for boiler plate starter code where I am maintaining this kind of code.

        You can find it here: 
        https://github.com/dyslexicanaboko/code-snippets/tree/develop/Visual%20C%23/BasicDataLayers
        """;

      _parentResultsWindow.AddTab("Basic data layers", content, string.Empty);
      _parentResultsWindow.Show();
    }
  }
}
