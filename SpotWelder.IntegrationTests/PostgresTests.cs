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
    var p = GetParameters();

    var svc = new QueryToClassService(
      new QueryToClassRepository(),
      new GeneralDatabaseQueries(),
      new CodeGenerationFactory(new[] { new ApiControllerGenerator() }));

    //Bad test - does it blow up?
    svc.Generate(p);

    Assert.Pass();
  }

  private QueryToClassParameters GetParameters()
  {
    var svcNameFormat = new NameFormatService();

    var obj = new QueryToClassParameters();

    obj.LanguageType = CodeType.CSharp;
    obj.OverwriteExistingFiles = true;
    obj.Namespace = "NoOneCares";
    obj.ServerConnection.SqlEngine = SqlEngine.SqlServer;
    obj.ServerConnection.TableQuery = svcNameFormat.ParseTableName("public.category");
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