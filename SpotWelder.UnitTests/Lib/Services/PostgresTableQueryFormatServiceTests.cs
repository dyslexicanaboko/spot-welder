using NUnit.Framework;
using SpotWelder.Lib.Services;
using System;

namespace SpotWelder.Tests.Lib.Services
{
  [TestFixture]
  public class PostgresTableQueryFormatServiceTests
  {
    [SetUp]
    public void Setup()
    {
      _service = new PostgresTableQueryFormatService();
    }

    private ITableQueryFormatService _service;

    [TestCase("a", "public.a")]
    [TestCase("a.b", "a.b")]
    [TestCase("\"a b\"", "public.\"a b\"")]
    [TestCase("a.\"b c\"", "a.\"b c\"")]
    [TestCase("\"a b\".\"c d\"", "\"a b\".\"c d\"")]
    [TestCase("\"a b\".c", "\"a b\".c")]
    public void Parse_different_table_names_successfully(string input, string expected)
    {
      //Arrange

      //Act
      var tq = _service.ParseTableName(input);

      var actual = _service.FormatTableQuery(tq);

      //Assert
      Assert.That(expected.Equals(actual, StringComparison.InvariantCultureIgnoreCase));
    }

    [TestCase("TableName", "TableName")]
    [TestCase("tablename", "tablename")]
    [TestCase("public.TableName", "TableName")]
    [TestCase("public.\"T a b l e N a m e\"", "TableName")]
    [TestCase("public.\"T A B L E N A M E\"", "TABLENAME")]
    [TestCase("\"T   a   b   l   e   N   a   m   e\"", "TableName")]
    public void Get_class_name(string input, string expected)
    {
      //Arrange
      var tq = _service.ParseTableName(input);

      //Act
      var actual = _service.GetClassName(tq);

      //Assert
      Assert.That(expected, Is.EqualTo(actual));
    }
  }
}
