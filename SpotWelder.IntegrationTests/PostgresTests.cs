using NUnit.Framework;
using SpotWelder.Lib;
using SpotWelder.Lib.DataAccess;
using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services;
using SpotWelder.Lib.Services.CodeFactory;
using SpotWelder.Lib.Services.Generators;

namespace SpotWelder.IntegrationTests;

[TestFixture]
public class PostgresTests 
  : SpotWelderTestBase
{
  [Test]
  public void HappyPathTest()
  {
    var svcNameFormat = new PostgresTableQueryFormatService();

    var p = GetParameters();
    p.ServerConnection.SqlEngine = SqlEngine.Postgres;
    p.ServerConnection.ConnectionString = "Host=localhost;Username=postgres;Password=postgres;Database=millions_of_things";
    p.ServerConnection.SourceSqlType = SourceSqlType.TableName;
    p.ServerConnection.SourceSqlText = "public.category";
    p.ServerConnection.TableQuery = svcNameFormat.ParseTableName(p.ServerConnection.SourceSqlText);

    var svc = new QueryToClassService(
      new QueryToClassRepository(),
      new GeneralDatabaseQueries(),
      new CodeGenerationFactory(new[] { new ApiControllerGenerator() }));

    //Bad test - does it blow up?
    var lst = svc.Generate(p);

    Assert.That(lst, Is.Not.Null);
    Assert.That(lst.Count, Is.GreaterThan(0));
  }

  private static QueryToClassParameters GetParameters()
  {

    var obj = new QueryToClassParameters();

    obj.LanguageType = CodeType.CSharp;
    obj.OverwriteExistingFiles = true;
    obj.Namespace = "NoOneCares";
    obj.SubjectName = "Category";
    obj.EntityName = "CategoryEntity";
    obj.ModelName = "CategoryModel";
    obj.Elections = GenerationElections.GenerateEntity;

    if (obj.Elections.HasAnyFlag(
          GenerationElections.MapModelToEntity,
          GenerationElections.MapEntityToModel,
          GenerationElections.MapInterfaceToEntity,
          GenerationElections.MapInterfaceToModel,
          GenerationElections.MapCreateModelToEntity,
          GenerationElections.MapPatchModelToEntity))
      obj.Elections |= GenerationElections.GenerateMapper;

    return obj;
  }
}