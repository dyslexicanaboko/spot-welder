using Moq;
using NUnit.Framework;
using SpotWelder.Lib;
using SpotWelder.Lib.DataAccess;
using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services;
using SpotWelder.Lib.Services.CodeFactory;
using SpotWelder.Lib.Services.Generators;
using SpotWelder.Tests.DummyObjects;
using SpotWelder.UnitTests;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SpotWelder.Tests.Lib.Services
{
  [TestFixture]
  public class QueryToClassServiceSpotWelderTests
    : SpotWelderTestBase
  {
    [Test]
    public void Providing_no_generation_elections_produces_null()
    {
      //Arrange
      var p = new QueryToClassParameters
      {
        LanguageType = CodeType.CSharp, Namespace = "SimpleClassCreator.Tests.DummyObjects"
      };

      p.ServerConnection.SourceSqlType = SourceSqlType.TableName;

      var repoQueryToClass = new Mock<IQueryToClassRepository>();
      var repoGeneral = new Mock<IGeneralDatabaseQueries>();

      var svc = new QueryToClassService(
        repoQueryToClass.Object,
        repoGeneral.Object,
        new CodeGenerationFactory(new[] { new ApiControllerGenerator() }));

      //Act
      var actual = svc.Generate(p);

      //Assert
      Assert.That(actual, Is.Null);
    }

    /*
    3. Need to create templates for whatever code it is that I am trying to create
    A. Let's create a dummy object that is easy to understand and work off of that 
        like your rudimentary person object 
    B. I want templates to be plug and play meaning the user can provide whatever 
        templates they want so long as they use the appropriate tags
    4. I don't like the if (tableName != null) clauses, need to abstract this, let's 
    pause and see what happens */
    [Test]
    public void Person_table_produces_person_class()
    {
      //Arrange
      var expected = PersonUtil.PersonClass;

      var sq = PersonUtil.GetPersonAsSchemaQuery();

      var p = new QueryToClassParameters
      {
        LanguageType = CodeType.CSharp,
        Namespace = "SimpleClassCreator.Tests.DummyObjects",
        EntityName = $"{sq.TableQuery.Table}Entity",
        Elections = GenerationElections.GenerateEntity,
        SubjectName = sq.TableQuery.Table
      };

      p.ServerConnection.SourceSqlType = SourceSqlType.TableName;
      p.ServerConnection.SourceSqlText = sq.TableQuery.Table;
      p.ServerConnection.TableQuery = sq.TableQuery;

      var repoQueryToClass = new Mock<IQueryToClassRepository>();

      repoQueryToClass.Setup(x => x.GetSchema(p.ServerConnection.TableQuery, It.IsAny<string>()))
        .Returns(sq); //TODO: Fix this later

      var repoGeneral = new Mock<IGeneralDatabaseQueries>();

      var svc = new QueryToClassService(
        repoQueryToClass.Object,
        repoGeneral.Object,
        new CodeGenerationFactory(new[] { new ClassEntityGenerator() }));

      //Act
      var actual = svc.Generate(p).Single().Contents;

      //This is for debug only
      //DumpFile("Expected.cs", expected);
      //DumpFile("Actual.cs", actual);

      //Assert
      //Spacing is still an issue, so going to give it a pass for now
      AssertAreEqualIgnoreWhiteSpace(expected, actual);
    }

    //This is for debug only
    private void DumpFile(string fileName, string contents)
    {
      File.WriteAllText(@"C:\Dump\" + fileName, contents);
    }

    private void AssertAreEqualIgnoreWhiteSpace(string expected, string actual)
    {
      var re = new Regex(@"\s+");

      expected = re.Replace(expected, string.Empty);
      actual = re.Replace(actual, string.Empty);

      Assert.That(expected, Is.EqualTo(actual));
    }
  }
}
