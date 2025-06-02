using SpotWelder.Lib;
using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;

namespace SpotWelder.UnitTests.Common.DummyObjects
{
  public class PersonEntity
  {
    public int PersonId { get; set; }

    public int Age { get; set; }

    public string FirstName { get; set; }

    public string MiddleName { get; set; }

    public string LastName { get; set; }

    public DateTime BirthDate { get; set; }
  }

  public static class PersonUtil
  {
    public const string PersonEntity =
      @"using System;

namespace SimpleClassCreator.Tests.DummyObjects
{

    public class PersonEntity
    {
        public int PersonId { get; set; }

        public int? Age { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public DateTime? BirthDate { get; set; }
    }
}
";

    public static SchemaQuery GetPersonAsSchemaQuery()
    {
      var sq = new SchemaQuery(
        SqlEngine.SqlServer,
        new TableQuery { Schema = "dbo", Table = nameof(DummyObjects.PersonEntity) },
        "Does not matter for QA purposes");

      sq.SetPrimaryKey(GetSchemaColumn(nameof(DummyObjects.PersonEntity.PersonId), typeof(int)));

      sq.ColumnsNoPk = new List<SchemaColumn>
      {
        GetSchemaColumn(nameof(DummyObjects.PersonEntity.Age), typeof(int), true),
        GetSchemaColumn(nameof(DummyObjects.PersonEntity.FirstName), typeof(string)),
        GetSchemaColumn(nameof(DummyObjects.PersonEntity.MiddleName), typeof(string), true),
        GetSchemaColumn(nameof(DummyObjects.PersonEntity.LastName), typeof(string)),
        GetSchemaColumn(nameof(DummyObjects.PersonEntity.BirthDate), typeof(DateTime), true)
      };

      //Order matters
      var lst = new List<SchemaColumn>();
      lst.Add(sq.PrimaryKey);
      lst.AddRange(sq.ColumnsNoPk);

      sq.ColumnsAll = lst;

      return sq;
    }

    private static SchemaColumn GetSchemaColumn(string columnName, Type type, bool isNullable = false)
    {
      var sqlEngine = SqlEngine.SqlServer;
      
      return new SchemaColumn(
        sqlEngine,
        columnName,
        type,
        sqlType: TypesService.GetTypeMapper(sqlEngine).GetSqlDataTypeAsString(type),
        isDbNullable: isNullable);
    }

    private static DataColumn GetNonNullColumn(string columnName, Type type)
    {
      var dc = new DataColumn(columnName, type);
      dc.AllowDBNull = false;

      return dc;
    }
  }

  /*
      This is not used by anything directly.
      I am hand crafting an object here to copy and paste it as a string for unit testing output comparison.
   */
  public class PersonService
  {
  }

  public class PersonRepository
  {
  }
}
