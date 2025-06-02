using NUnit.Framework;
using SpotWelder.IntegrationTests.Common;
using SpotWelder.Lib;
using SpotWelder.Lib.DataAccess;
using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services;
using SpotWelder.Lib.Services.CodeFactory;
using SpotWelder.Lib.Services.Generators;
using SpotWelder.Lib.Services.TableQueryFormats;

namespace SpotWelder.IntegrationTests;

[TestFixture]
public class ModelParityTests 
  : SpotWelderTestBase
{

  private QueryToClassServiceSetup _svc;

  public void Setup()
  {
    _svc = new QueryToClassServiceSetup();  
  }

  [Test]
  public void ModelParity_ClassEntity(SqlEngine sqlEngine)
  {
    //Get parameters for the SQL engine
    var p = new QueryToClassServiceSetup().GetParameters(sqlEngine);

    //Iterate through the elections one at a time
    p.Elections = GenerationElections.GenerateEntity;

    //Set the corresponding generator per election
    var svc = _svc.GetQueryToClassService(p);

    //Generate the class entity
    var lst = svc.Generate(p);

    //Perform comparisons on the expected output versus the generated output
    Assert.That(lst, Is.Not.Null);
    Assert.That(lst.Count, Is.GreaterThan(0));
  }
}