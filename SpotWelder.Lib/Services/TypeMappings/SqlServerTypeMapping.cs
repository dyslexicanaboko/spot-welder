using System;
using System.Collections.Generic;
using System.Data;

namespace SpotWelder.Lib.Services.TypeMappings
{
  public class SqlServerTypeMapping : TypeMappingBase
  {
    //FYI:The enumerations below have a proper set of mappings going from Sql to System
    /*  System.Data.SqlDbType
            SQL Server specific
            C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Data.dll
        
        System.Data.DbType
            ANSI SQL (Generic)
            C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\5.0.0\ref\net5.0\System.Data.Common.dll */

    /// <inheritdoc />
    public override string GetSqlDataTypeAsString(Type type) => MapSystemToSqlLoose[type].ToString();

    /// <inheritdoc />
    public override DbType GetDbTypeBySqlDataType(Enum sqlDataType)
      => MapSqlDbTypeToDbTypeLoose[(SqlDbType)sqlDataType];

    /// <inheritdoc />
    public override Enum GetSqlDataType(string sqlDataTypeName) => SqlTypes[sqlDataTypeName];

    /// <summary>
    ///   Loose mapping going from System type to Sql Server database type.
    /// </summary>
    private static readonly Dictionary<Type, SqlDbType> MapSystemToSqlLoose = new()
    {
      { typeof(bool), SqlDbType.Bit },
      { typeof(byte), SqlDbType.TinyInt },
      { typeof(short), SqlDbType.SmallInt },
      { typeof(int), SqlDbType.Int },
      { typeof(long), SqlDbType.BigInt },
      { typeof(string), SqlDbType.NVarChar }, //Could be Char, NChar or VarChar
      { typeof(char[]), SqlDbType.NVarChar }, //Could be Char, NChar or VarChar
      { typeof(byte[]), SqlDbType.VarBinary }, //Could be Binary
      { typeof(decimal), SqlDbType.Decimal },
      { typeof(float), SqlDbType.Real }, //System.Single -> float -> SqlDbType.Real
      { typeof(double), SqlDbType.Float }, //Do not confuse with System.float
      { typeof(TimeSpan), SqlDbType.Time },
      { typeof(DateTime), SqlDbType.DateTime2 },
      { typeof(DateTimeOffset), SqlDbType.DateTimeOffset },
      { typeof(Guid), SqlDbType.UniqueIdentifier }
    };

    /// <summary>
    ///   Loose mapping going from SQL Server database type to Database type. Does not account for all types!
    /// </summary>
    private static readonly Dictionary<SqlDbType, DbType> MapSqlDbTypeToDbTypeLoose = new()
    {
      { SqlDbType.BigInt, DbType.Int64 },

      //{ SqlDbType.Binary, ??? },
      { SqlDbType.Bit, DbType.Boolean },
      { SqlDbType.Char, DbType.AnsiStringFixedLength },
      { SqlDbType.Date, DbType.Date },
      { SqlDbType.DateTime, DbType.DateTime },
      { SqlDbType.DateTime2, DbType.DateTime2 },
      { SqlDbType.DateTimeOffset, DbType.DateTimeOffset },
      { SqlDbType.Decimal, DbType.Decimal },
      { SqlDbType.Float, DbType.Double },
      { SqlDbType.Int, DbType.Int32 },
      { SqlDbType.Money, DbType.Currency },
      { SqlDbType.NChar, DbType.StringFixedLength },
      { SqlDbType.NVarChar, DbType.String },
      { SqlDbType.Real, DbType.Single },
      { SqlDbType.SmallInt, DbType.Int16 },
      { SqlDbType.Time, DbType.Time },
      { SqlDbType.TinyInt, DbType.Byte },
      { SqlDbType.UniqueIdentifier, DbType.Guid },
      { SqlDbType.VarBinary, DbType.Binary },
      { SqlDbType.VarChar, DbType.AnsiString },
      { SqlDbType.Xml, DbType.Xml }
    };

    /// <summary>
    ///   Strong mapping of Sql Server Database type lower case names to their equivalent Enumeration.
    /// </summary>
    private static readonly Dictionary<string, SqlDbType> SqlTypes = Utils.GetEnumDictionary<SqlDbType>(true);
  }
}
