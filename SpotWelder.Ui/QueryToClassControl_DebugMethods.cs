using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotWelder.Ui
{
  /// <summary>
  ///   Interaction logic for QueryToClassControl.xaml
  /// </summary>
  public partial class QueryToClassControl
  {
    private void DebugMinimalPostgresTest()
    {
#if DEBUG
      ConnectionStringCb.DebugSetPostgresTestParameters();
      TxtSourceSqlText.Text = "public.task";
      TxtNamespaceName.Text = "Namespace1";
      TxtEntityName.Text = "Task";
      TxtClassEntityName.Text = "TaskEntity";

      CbClassEntity.IsChecked = true;
      CbClassModel.IsChecked = false; //Selected by default, so un-checking
      CbRepoDapper.IsChecked = true;
      CbRepoStatic.IsChecked = true;
      CbMakeAsynchronous.IsChecked = true;
#endif
    }

    private void DebugWholeSqlServerTest()
    {
#if DEBUG
      ConnectionStringCb.DebugSetSqlServerTestParameters();

      RbSourceTypeTableName.IsChecked = true;
      RbSourceTypeQuery.IsChecked = false;

      TxtSourceSqlText.Text = "dbo.Task";
      TxtNamespaceName.Text = "Namespace1";
      TxtEntityName.Text = "Task";
      TxtClassEntityName.Text = "TaskEntity";
      TxtClassModelName.Text = "TaskV1Model";

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
      CbClassCreatedModel.IsChecked = true;
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
      CbMapInterfaceToModel.IsChecked = false;
      CbMapInterfaceToEntity.IsChecked = false;
      CbMapEntityToModel.IsChecked = true;
      CbMapModelToEntity.IsChecked = true;
      CbMapCreateModelToEntity.IsChecked = true;
      CbMapPatchModelToEntity.IsChecked = true;
      CbMapEntityToCreatedModel.IsChecked = true;
#endif
    }

    private void DebugWholeSqlServerTestForParity()
    {
#if DEBUG
      ConnectionStringCb.DebugSetSqlServerParityTestParameters();

      RbSourceTypeTableName.IsChecked = true;
      RbSourceTypeQuery.IsChecked = false;

      TxtSourceSqlText.Text = "[dbo].[DataTypeTest]";
      TxtNamespaceName.Text = "Namespace1";
      TxtEntityName.Text = "DataTypeTest";
      TxtClassEntityName.Text = "DataTypeTestEntity";
      TxtClassModelName.Text = "DataTypeTestModel";

      //Repository
      CbRepoStatic.IsChecked = true;
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
      CbMakeAsynchronous.IsChecked = false;
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

    private void DebugWholePostgresTestForParity()
    {
#if DEBUG
      ConnectionStringCb.DebugSetPostgresParityTestParameters();

      RbSourceTypeTableName.IsChecked = true;
      RbSourceTypeQuery.IsChecked = false;

      TxtSourceSqlText.Text = "public.data_type_test";
      TxtNamespaceName.Text = "Namespace1";
      TxtEntityName.Text = "DataTypeTest";
      TxtClassEntityName.Text = "DataTypeTestEntity";
      TxtClassModelName.Text = "DataTypeTestModel";

      //Repository
      CbRepoStatic.IsChecked = true;
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
      CbMakeAsynchronous.IsChecked = false;
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
  }
}
