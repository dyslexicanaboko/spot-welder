using NUnit.Framework;
using SpotWelder.IntegrationTests.Common;
using SpotWelder.Lib;
using System.Linq;

namespace SpotWelder.IntegrationTests;

[TestFixture]
public class ModelParityTests 
  : SpotWelderTestBase
{
  private QueryToClassServiceSetup _svc;
  private TestContainer _testContainer;

  [SetUp]
  public void Setup()
  {
    _svc = new QueryToClassServiceSetup();
    _testContainer = new TestContainer();
  }

  //I will worry about these later
  //[TestCase(GenerationElections.GenerateEntityAsTypeScript)]
  //[TestCase(GenerationElections.GenerateEntityAsJavaScript)]

  [TestCase(GenerationElections.Entity)]
  [TestCase(GenerationElections.EntityEqualityComparer)]
  [TestCase(GenerationElections.Model)]
  [TestCase(GenerationElections.CreateModel)]
  [TestCase(GenerationElections.PatchModel)]
  [TestCase(GenerationElections.Interface)]
  [TestCase(GenerationElections.SerializeCsv)]
  [TestCase(GenerationElections.SerializeJson)]
  [TestCase(GenerationElections.RepoStatic)]
  [TestCase(GenerationElections.RepoDapper)]
  [TestCase(GenerationElections.Manager)]
  [TestCase(GenerationElections.ApiController)]
  [TestCase(GenerationElections.Mapper)]
  public void ModelParity_SqlServer_Asynchronous(GenerationElections election)
    => ModelParity_BaseTest(election, SqlEngine.SqlServer, true);

  [TestCase(GenerationElections.Entity)]
  [TestCase(GenerationElections.EntityEqualityComparer)]
  [TestCase(GenerationElections.Model)]
  [TestCase(GenerationElections.CreateModel)]
  [TestCase(GenerationElections.PatchModel)]
  [TestCase(GenerationElections.Interface)]
  [TestCase(GenerationElections.SerializeCsv)]
  [TestCase(GenerationElections.SerializeJson)]
  [TestCase(GenerationElections.RepoStatic)]
  [TestCase(GenerationElections.RepoDapper)]
  [TestCase(GenerationElections.Manager)]
  [TestCase(GenerationElections.ApiController)]
  [TestCase(GenerationElections.Mapper)]
  public void ModelParity_SqlServer_Synchronous(GenerationElections election)
    => ModelParity_BaseTest(election, SqlEngine.SqlServer, false);

  /*
    The casing that is generated for Postgres C# properties is different than SQL Server
    because Postgres uses snake_case while SQL Server uses PascalCase. Therefore, the
    subjectiveness of the casing arises and there is no fool proof way to make sure there
    is a "correct" match. Examples:
      bytearray_binary  -> BytearrayBinary if using Humanizer
        There is no delimiter between byte and array, so array is not capitalized.
      bytearray_binary  -> ByteArrayBinary if using Custom String Segmentation
        My custom code handles it well because the individual words are matched by US English nicely.
      data_type_test_id -> DataTypeTestId if using Humanizer
        This example has no problem as there are delimiters between all words.
      data_type_test_id -> DatatypeTestId if using Custom String Segmentation
        My custom code fails because there is legimately a word named "Datatype" in US English. However,
        I subjectively want it to say "DataType". This cannot be solved with a simple code.
   */

  [TestCase(GenerationElections.RepoStatic)]
  [TestCase(GenerationElections.RepoDapper)]
  public void ModelParity_Postgres_Asynchronous(GenerationElections election)
   => ModelParity_BaseTest(election, SqlEngine.Postgres, true);

  [TestCase(GenerationElections.RepoStatic)]
  [TestCase(GenerationElections.RepoDapper)]
  public void ModelParity_Postgres_Synchronous(GenerationElections election)
   => ModelParity_BaseTest(election, SqlEngine.Postgres, false);

  private void ModelParity_BaseTest(GenerationElections election, SqlEngine sqlEngine, bool isAsync)
  {
    //Get parameters for the SQL engine
    var p = new QueryToClassServiceSetup().GetParameters(sqlEngine, isAsync, election);

    //Set the corresponding generator per election
    var svc = _svc.GetQueryToClassService(_testContainer.ServiceProvider, p);

    var expected = _svc.GetExpectedResult(sqlEngine, isAsync, election);

    //Generate the class entity
    var lst = svc.Generate(p);

    Assert.That(lst, Is.Not.Null);
    Assert.That(lst.Count, Is.GreaterThan(0));

    //Have to match the result to what was passed in.
    var actual = lst.Single(x => x.Election == election).Contents;

    //For debugging purposes only - do not delete
    //DumpToFile(expected, actual);

    //Perform comparisons on the expected output versus the generated output
    AssertAreEqualIgnoreWhiteSpace(expected, actual);
  }
}