using NUnit.Framework;
using SpotWelder.Lib;
using SpotWelder.Lib.DataAccess;
using SpotWelder.Lib.Models;
using SpotWelder.UnitTests;

namespace SpotWelder.Tests.Lib.DataAccess
{
  [TestFixture]
  public class QueryToClassRepositorySpotWelderTests
    : SpotWelderTestBase
  {
    [Test]
    public void BadTest()
    {
      var repo = new QueryToClassRepository();

      var con = new ServerConnection
      {
        SqlEngine = SqlEngine.SqlServer,
        ConnectionString = "Server=.;Database=ScratchSpace;Integrated Security=SSPI;Encrypt=False;",
        SourceSqlType = SourceSqlType.Query,
        SourceSqlText = """
        SET FMTONLY ON; 
        SELECT * FROM dbo.NumberCollection; 
        SET FMTONLY OFF;
        """,
        TableQuery = new TableQuery { Schema = "dbo", Table = "NumberCollection" }
      };

      repo.ConfigureSqlClient(con);

      var dt = repo.GetSchema(con.TableQuery, con.SourceSqlType, con.SourceSqlText);

      Assert.Pass();
    }
  }
}
