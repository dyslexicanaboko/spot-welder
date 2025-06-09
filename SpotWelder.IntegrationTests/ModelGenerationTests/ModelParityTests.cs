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

  [TestCase(GenerationElections.GenerateEntity)]
  [TestCase(GenerationElections.GenerateEntityEqualityComparer)]
  [TestCase(GenerationElections.GenerateModel)]
  [TestCase(GenerationElections.GenerateCreateModel)]
  [TestCase(GenerationElections.GeneratePatchModel)]
  [TestCase(GenerationElections.GenerateInterface)]
  [TestCase(GenerationElections.SerializeCsv)]
  [TestCase(GenerationElections.SerializeJson)]
  [TestCase(GenerationElections.RepoStatic)]
  [TestCase(GenerationElections.RepoDapper)]
  [TestCase(GenerationElections.Service)]
  [TestCase(GenerationElections.ApiController)]
  [TestCase(GenerationElections.GenerateMapper)]
  public void ModelParity_SqlServer_AsAsync(GenerationElections election)
  {
    bool makeAsync = true;

    //Get parameters for the SQL engine
    var p = new QueryToClassServiceSetup().GetParameters(SqlEngine.SqlServer, makeAsync, election);

    //Set the corresponding generator per election
    var svc = _svc.GetQueryToClassService(_testContainer.ServiceProvider, p);

    var expected = _svc.GetExpectedResult(SqlEngine.SqlServer, makeAsync, election);

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

  [TestCase(GenerationElections.GenerateEntity)]
  [TestCase(GenerationElections.GenerateEntityEqualityComparer)]
  [TestCase(GenerationElections.GenerateModel)]
  [TestCase(GenerationElections.GenerateCreateModel)]
  [TestCase(GenerationElections.GeneratePatchModel)]
  [TestCase(GenerationElections.GenerateInterface)]
  [TestCase(GenerationElections.SerializeCsv)]
  [TestCase(GenerationElections.SerializeJson)]
  [TestCase(GenerationElections.RepoStatic)]
  [TestCase(GenerationElections.RepoDapper)]
  [TestCase(GenerationElections.Service)]
  [TestCase(GenerationElections.ApiController)]
  [TestCase(GenerationElections.GenerateMapper)]
  public void ModelParity_SqlServer_AsSync(GenerationElections election)
  {
    bool makeAsync = false;
    
    //Get parameters for the SQL engine
    var p = new QueryToClassServiceSetup().GetParameters(SqlEngine.SqlServer, makeAsync, election);

    //Set the corresponding generator per election
    var svc = _svc.GetQueryToClassService(_testContainer.ServiceProvider, p);

    var expected = _svc.GetExpectedResult(SqlEngine.SqlServer, makeAsync, election);

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