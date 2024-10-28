using SpotWelder.Lib.Services;
using System;
using System.Reflection;

namespace SpotWelder.Lib.Models
{
  public class SchemaColumn
  {
    public SchemaColumn(
      SqlEngine sqlEngine,
      string columnName,
      Type systemType,
      string sqlType,
      bool isPrimaryKey = false,
      bool isIdentity = false,
      bool isDbNullable = false,
      int size = 0,
      int precision = 0,
      int scale = 0)
    {
      SqlEngine = sqlEngine;
      IsPrimaryKey = isPrimaryKey;
      IsIdentity = isIdentity;
      IsDbNullable = isDbNullable;
      ColumnName = columnName;
      SystemType = systemType;
      SqlType = sqlType;
      Size = size;
      Precision = precision;
      Scale = scale;
    }

    public SchemaColumn(SqlEngine sqlEngine, PropertyInfo property)
    {
      var underlyingType = Nullable.GetUnderlyingType(property.PropertyType);

      //Just checking if it's a value type is not enough, as it could be a nullable value type
      IsDbNullable = underlyingType != null;

      ColumnName = property.Name;

      SystemType = underlyingType ?? property.PropertyType;

      SqlEngine = sqlEngine;

      //This is in good faith, but it's not 100% accurate
      SqlType = TypesService.MapSystemToSqlLoose[SystemType].ToString();

      IsPrimaryKey = false;
      IsIdentity = false;
      Size = 0;
      Precision = 0;
      Scale = 0;
    }

    /// <summary> In order to use the right type mappings, the origination of where this column came from is required. </summary>
    public SqlEngine SqlEngine { get; set; }

    public bool IsPrimaryKey { get; set; }

    public bool IsIdentity { get; set; }

    public bool IsDbNullable { get; set; }

    public string ColumnName { get; set; }

    public Type SystemType { get; set; }

    /// <summary> String representation of the SQL engine's Data Type. </summary>
    public string SqlType { get; set; } //This cannot be an enumeration because it depends on the SQL engine

    public int Size { get; set; }

    public int Precision { get; set; }

    public int Scale { get; set; }
  }
}
