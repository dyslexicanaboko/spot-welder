namespace SpotWelder.Ui
{
  /// <summary>
  ///   Interaction logic for QueryToClassControl.xaml
  /// </summary>
  public partial class QueryToClassControl
  {
    private void DebugMinimalPostgresTest()
    {
      ConnectionStringCb.DebugSetPostgresTestParameters();
      TxtSourceSqlText.Text = "public.task";
      TxtNamespaceName.Text = "Namespace1";
      TxtSubjectName.Text = "Task";
      TxtClassEntityName.Text = "TaskEntity";

      CbClassEntity.IsChecked = true;
      CbClassModel.IsChecked = false; //Selected by default, so un-checking
      CbRepoDapper.IsChecked = true;
      CbRepoStatic.IsChecked = true;
      CbMakeAsynchronous.IsChecked = true;
    }

    private void DebugWholeSqlServerTest()
    {
      ConnectionStringCb.DebugSetSqlServerTestParameters();

      RbSourceTypeTableName.IsChecked = true;
      RbSourceTypeQuery.IsChecked = false;

      TxtSourceSqlText.Text = "dbo.Task";
      TxtNamespaceName.Text = "Namespace1";
      TxtSubjectName.Text = "Task";
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
      CbManager.IsChecked = true;

      //Mappings
      CbMapEntityToModel.IsChecked = true;
      CbMapModelToEntity.IsChecked = true;
      CbMapCreateModelToEntity.IsChecked = true;
      CbMapPatchModelToEntity.IsChecked = true;
      CbMapEntityToCreatedModel.IsChecked = true;
    }

    private void DebugWholeSqlServerTestForParity()
    {
      ConnectionStringCb.DebugSetSqlServerParityTestParameters();

      RbSourceTypeTableName.IsChecked = true;
      RbSourceTypeQuery.IsChecked = false;

      TxtSourceSqlText.Text = "[dbo].[DataTypeTest]";
      TxtNamespaceName.Text = "Namespace1";
      TxtSubjectName.Text = "DataTypeTest";
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
      CbManager.IsChecked = true;

      //Mappings
      CbMapEntityToModel.IsChecked = true;
      CbMapModelToEntity.IsChecked = true;
      CbMapCreateModelToEntity.IsChecked = true;
      CbMapPatchModelToEntity.IsChecked = true;
    }

    private void DebugWholePostgresTestForParity()
    {
      ConnectionStringCb.DebugSetPostgresParityTestParameters();

      RbSourceTypeTableName.IsChecked = true;
      RbSourceTypeQuery.IsChecked = false;

      TxtSourceSqlText.Text = "public.data_type_test";
      TxtNamespaceName.Text = "Namespace1";
      TxtSubjectName.Text = "DataTypeTest";
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
      CbManager.IsChecked = true;

      //Mappings
      CbMapEntityToModel.IsChecked = true;
      CbMapModelToEntity.IsChecked = true;
      CbMapCreateModelToEntity.IsChecked = true;
      CbMapPatchModelToEntity.IsChecked = true;
    }

    private void DebugCompoundQuerySqlServerTest()
    {
      //This needs to be `InStock`
      ConnectionStringCb.DebugSetSqlServerTestParameters();

      RbSourceTypeTableName.IsChecked = false;
      RbSourceTypeQuery.IsChecked = true;
      TxtSourceSqlText.Text = """
                              SELECT
                              	 s.StockId
                              	,s.Symbol
                              	,S.[Name]
                              	,s.CreateOnUtc AS StockCreatedOn
                              	,s.Notes
                              	,s.UpdatedOnUtc AS StockUpdatedOn
                              	,q.QuoteId
                              	,q.[Date] AS QuoteDate
                              	,q.Price
                              	,q.Volume
                              	,q.CreatedOnUtc AS QuoteCreatedOn
                              FROM dbo.Stock s
                              	INNER JOIN dbo.Quote q
                              		ON s.StockId = q.StockId
                              """;
      TxtNamespaceName.Text = "Namespace2";
      TxtSubjectName.Text = "StockQuote";
      TxtClassEntityName.Text = "StockQuoteEntity";
      TxtClassModelName.Text = "StockQuoteV1Model";

      CbRepoDapper.IsChecked = true;

      //Entity
      CbClassEntity.IsChecked = true;
      CbClassEntityIEquatable.IsChecked = false;
      CbClassEntityIComparable.IsChecked = false;

      //Interface
      CbClassInterface.IsChecked = false;

      //Models
      CbClassModel.IsChecked = true;
      CbClassCreateModel.IsChecked = false;
      CbClassCreatedModel.IsChecked = false;
      CbClassPatchModel.IsChecked = false;

      //Services
      CbClassEntityEqualityComparer.IsChecked = false;
      CbSerializeCsv.IsChecked = false;
      CbSerializeJson.IsChecked = false;
      CbValidation.IsChecked = true;

      //Layers
      CbMakeAsynchronous.IsChecked = true;
      CbApiController.IsChecked = true;
      CbManager.IsChecked = true;

      //Mappings
      CbMapEntityToModel.IsChecked = true;
      CbMapModelToEntity.IsChecked = false;
      CbMapCreateModelToEntity.IsChecked = false;
      CbMapPatchModelToEntity.IsChecked = false;
      CbMapEntityToCreatedModel.IsChecked = false;
    }

    private void DebugOneTableSqlServerTest()
    {
      //This needs to be `InStock`
      ConnectionStringCb.DebugSetSqlServerTestParameters();

      RbSourceTypeTableName.IsChecked = true;
      RbSourceTypeQuery.IsChecked = false;
      TxtSourceSqlText.Text = "dbo.Stock";
      TxtNamespaceName.Text = "Namespace2";
      TxtSubjectName.Text = "Stock";
      TxtClassEntityName.Text = "StockEntity";
      TxtClassModelName.Text = "StockModel";

      CbRepoDapper.IsChecked = true;

      //Entity
      CbClassEntity.IsChecked = true;
      CbClassEntityIEquatable.IsChecked = false;
      CbClassEntityIComparable.IsChecked = false;

      //Interface
      CbClassInterface.IsChecked = false;

      //Models
      CbClassModel.IsChecked = true;
      CbClassCreateModel.IsChecked = true;
      CbClassCreatedModel.IsChecked = true;
      CbClassPatchModel.IsChecked = true;

      //Services
      CbClassEntityEqualityComparer.IsChecked = false;
      CbSerializeCsv.IsChecked = false;
      CbSerializeJson.IsChecked = false;
      CbValidation.IsChecked = true;

      //Layers
      CbMakeAsynchronous.IsChecked = true;
      CbApiController.IsChecked = true;
      CbManager.IsChecked = true;

      //Mappings
      CbMapEntityToModel.IsChecked = true;
      CbMapModelToEntity.IsChecked = false;
      CbMapCreateModelToEntity.IsChecked = true;
      CbMapPatchModelToEntity.IsChecked = true;
      CbMapEntityToCreatedModel.IsChecked = true;
      CbMapEntityToRecord
    }
  }
}
