using SpotWelder.Lib;
using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services;
using SpotWelder.UnitTests.Common;
using System;

namespace SpotWelder.UnitTests
{
  public abstract class SpotWelderTestBase
    : TestBase
  {
    protected SchemaColumn GetSchemaColumn(Type type, bool isNullable)
    {
      var sqlEngine = SqlEngine.SqlServer;

      return new SchemaColumn(

        sqlEngine,
        "DoesNotMatter",
        type,
        TypesService.GetTypeMapper(sqlEngine).GetSqlDataTypeAsString(type),
        isDbNullable: isNullable
      );
    }
  }
}
