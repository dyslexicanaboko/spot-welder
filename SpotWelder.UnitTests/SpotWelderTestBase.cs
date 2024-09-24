using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services;
using SpotWelder.Tests.Common;
using System;

namespace SpotWelder.UnitTests
{
  public abstract class SpotWelderTestBase
    : TestBase
  {
    protected SchemaColumn GetSchemaColumn(Type type, bool isNullable)
    {
      var c = new SchemaColumn
      {
        ColumnName = "DoesNotMatter",
        SystemType = type,
        IsDbNullable = isNullable,
        SqlType = TypesService.MapSystemToSqlLoose[type].ToString()
      };

      return c;
    }
  }
}
