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
      TxtRootContainingNamespace.Text = "Namespace1";
      TxtSubjectName.Text = "Task";

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
      TxtRootContainingNamespace.Text = "Namespace1";
      TxtSubjectName.Text = "Task";

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
      TxtRootContainingNamespace.Text = "Namespace1";
      TxtSubjectName.Text = "DataTypeTest";

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
      TxtRootContainingNamespace.Text = "Namespace1";
      TxtSubjectName.Text = "DataTypeTest";

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
      TxtRootContainingNamespace.Text = "Namespace2";
      TxtSubjectName.Text = "StockQuote";

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
      TxtRootContainingNamespace.Text = "Namespace2";
      TxtSubjectName.Text = "Stock";
      RbArchitectureNTier.IsChecked = false;
      RbArchitectureFeatureBased.IsChecked = true;

      CbRepoDapper.IsChecked = true;

      //Entity
      CbClassEntity.IsChecked = true;
      CbClassEntityIEquatable.IsChecked = true;
      CbClassEntityIComparable.IsChecked = true;
      CbClassEntityEqualityComparer.IsChecked = true;

      //Interface
      CbClassInterface.IsChecked = false;

      //Models
      CbClassModel.IsChecked = true;
      CbClassCreateModel.IsChecked = true;
      CbClassCreatedModel.IsChecked = true;
      CbClassPatchModel.IsChecked = true;

      //Services
      CbValidation.IsChecked = true;

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
  }
}
