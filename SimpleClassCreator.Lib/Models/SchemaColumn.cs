using System;
using System.Reflection;
using SpotWelder.Lib.Services;

namespace SpotWelder.Lib.Models
{
  public class SchemaColumn
  {
    public SchemaColumn()
    {
    }

    public SchemaColumn(PropertyInfo property)
    {
      var underlyingType = Nullable.GetUnderlyingType(property.PropertyType);

      //Just checking if it's a value type is not enough, as it could be a nullable value type
      IsDbNullable = underlyingType != null;

      ColumnName = property.Name;

      SystemType = underlyingType ?? property.PropertyType;

      //This is in good faith, but it's not 100% accurate
      SqlType = TypesService.MapSystemToSqlLoose[SystemType].ToString();
    }

    public bool IsPrimaryKey { get; set; }

    public bool IsIdentity { get; set; }

    public bool IsDbNullable { get; set; }

    public string ColumnName { get; set; }

    public Type SystemType { get; set; }

    public string SqlType { get; set; }

    public int Size { get; set; }

    public int Precision { get; set; }

    public int Scale { get; set; }
  }
}
