using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
    public override Enum GetDbTypeAsSqlDataTypeEnum(DbType type)
      => MapDbTypeToSqlDbTypeLoose[type];

    /// <inheritdoc />
    public override DbType GetDbType(Enum sqlDataType)
      => MapSqlDbTypeToDbTypeLoose[(SqlDbType)sqlDataType];

    /// <inheritdoc />
    public override DbType GetDbType(string sqlDataTypeName) => GetDbType(SqlTypes[sqlDataTypeName]);

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
      //The higher up in the dictionary, the higher the precedence for conversion from DbType to SqlDbType.

      //Boolean      
      { SqlDbType.Bit, DbType.Boolean },

      //Integers
      { SqlDbType.TinyInt, DbType.Byte },
      { SqlDbType.SmallInt, DbType.Int16 },
      { SqlDbType.Int, DbType.Int32 },
      { SqlDbType.BigInt, DbType.Int64 },
      
      //Binary
      { SqlDbType.Binary, DbType.Binary }, //Binary A
      { SqlDbType.VarBinary, DbType.Binary }, //Binary B - no VarBrinary equivalent
      
      //Strings
      { SqlDbType.Char, DbType.AnsiStringFixedLength },
      { SqlDbType.VarChar, DbType.AnsiString },
      { SqlDbType.NChar, DbType.StringFixedLength },
      { SqlDbType.NVarChar, DbType.String },

      //Date and Time
      { SqlDbType.Date, DbType.Date },
      { SqlDbType.DateTime, DbType.DateTime },
      { SqlDbType.DateTime2, DbType.DateTime2 },
      { SqlDbType.DateTimeOffset, DbType.DateTimeOffset },
      { SqlDbType.SmallDateTime , DbType.DateTime },
      { SqlDbType.Time, DbType.Time },
      
      //Floating points
      { SqlDbType.Decimal, DbType.Decimal },
      { SqlDbType.Float, DbType.Double },
      { SqlDbType.Money, DbType.Currency },
      { SqlDbType.SmallMoney, DbType.Currency },
      { SqlDbType.Real, DbType.Single },
      
      //Special types
      { SqlDbType.UniqueIdentifier, DbType.Guid },
      { SqlDbType.Timestamp, DbType.Object }, // No equivalent
      { SqlDbType.Xml, DbType.Xml }
    };
        
    private static readonly Dictionary<DbType, SqlDbType> MapDbTypeToSqlDbTypeLoose =
      MapDbTypeToSqlDbTypeLooseSafe();

    private static Dictionary<DbType, SqlDbType> MapDbTypeToSqlDbTypeLooseSafe()
    {
      var dict = new Dictionary<DbType, SqlDbType>();

      //The problem with this method is that it will not account for all types
      //especially when there are duplicate dbType, so it's first in wins.
      foreach (var (sqlType, dbType) in MapSqlDbTypeToDbTypeLoose)
      {
        dict.TryAdd(dbType, sqlType);
      }

      return dict;
    }
    
    /// <summary>
    ///   Strong mapping of Sql Server Database type lower case names to their equivalent Enumeration.
    /// </summary>
    private static readonly Dictionary<string, SqlDbType> SqlTypes = Utils.GetEnumDictionary<SqlDbType>(true);
  }
}
