﻿using NUnit.Framework;
using SpotWelder.Lib.DataAccess;
using SpotWelder.Lib.Models;

namespace SpotWelder.Tests.Lib.DataAccess
{
    [TestFixture]
    public class QueryToClassRepositoryTests
        : TestBase
    {
        [Test]
        public void BadTest()
        {
            var repo = new QueryToClassRepository();
            
            repo.ChangeConnectionString("Server=.;Database=ScratchSpace;Integrated Security=SSPI;");

            var tq = new TableQuery { Schema = "dbo", Table = "NumberCollection" };

            var dt = repo.GetSchema(tq, "SET FMTONLY ON; SELECT * FROM dbo.NumberCollection; SET FMTONLY OFF;");

            Assert.Pass();
        }
    }
}
