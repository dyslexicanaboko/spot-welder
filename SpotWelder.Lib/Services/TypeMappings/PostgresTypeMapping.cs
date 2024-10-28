using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;

namespace SpotWelder.Lib.Services.TypeMappings
{
  public class PostgresTypeMapping : TypeMappingBase
  {
    /*
      https://www.npgsql.org/doc/types/basic.html -- official mapping of Postgres to .NET C# types
      https://stackoverflow.com/a/41275216/603807 -- user specified mapping

      NpgsqlTypes.NpgsqlDbType
        Postgres specific
        ~\.nuget\packages\npgsql\8.0.4\lib\net8.0\Npgsql.dll
      
      System.Data.DbType
        ANSI SQL (Generic)
        C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\5.0.0\ref\net5.0\System.Data.Common.dll */

    /// <inheritdoc />
    public override string GetSqlDataTypeAsString(Type type) => MapSystemToSqlLoose[type].ToString();

    /// <inheritdoc />
    public override DbType GetDbType(Enum sqlDataType)
      => MapSqlDbTypeToDbTypeLoose[(NpgsqlDbType)sqlDataType];

    /// <inheritdoc />
    public override DbType GetDbType(string sqlDataTypeName) => GetDbType(SqlTypes[sqlDataTypeName]);

    /// <summary>
    ///   Loose mapping going from System type to Sql Server database type.
    /// </summary>
    private static readonly Dictionary<Type, NpgsqlDbType> MapSystemToSqlLoose = new()
    {
      { typeof(bool), NpgsqlDbType.Boolean },
      { typeof(byte), NpgsqlDbType.Integer },
      { typeof(short), NpgsqlDbType.Integer },
      { typeof(int), NpgsqlDbType.Integer },
      { typeof(long), NpgsqlDbType.Bigint },
      { typeof(string), NpgsqlDbType.Varchar }, //More than likely Character Varying
      { typeof(char[]), NpgsqlDbType.Varchar },
      { typeof(byte[]), NpgsqlDbType.Bytea },
      { typeof(decimal), NpgsqlDbType.Numeric }, //Could be Money
      { typeof(float), NpgsqlDbType.Real }, //System.Single -> float -> NpgsqlDbType.Real
      { typeof(double), NpgsqlDbType.Double }, //Do not confuse with System.float
      { typeof(TimeSpan), NpgsqlDbType.Interval },
      { typeof(DateTime), NpgsqlDbType.Date }, //This may not be correct
      { typeof(DateTimeOffset), NpgsqlDbType.TimestampTz }, //Timestamp with time zone
      { typeof(Guid), NpgsqlDbType.Uuid }
    };

    /// <summary>
    ///   Loose mapping going from SQL Server database type to Database type. Does not account for all types!
    /// </summary>
    private static readonly Dictionary<NpgsqlDbType, DbType> MapSqlDbTypeToDbTypeLoose = new()
    {
      { NpgsqlDbType.Bigint, DbType.Int64 },

      //{ NpgsqlDbType.Binary, ??? },
      { NpgsqlDbType.Boolean, DbType.Boolean },
      { NpgsqlDbType.Char, DbType.AnsiStringFixedLength },
      { NpgsqlDbType.Date, DbType.Date },
      { NpgsqlDbType.Timestamp, DbType.DateTime },  //Guessing
      { NpgsqlDbType.Timestamp, DbType.DateTime2 }, //Guessing
      { NpgsqlDbType.TimestampTz, DbType.DateTimeOffset },
      { NpgsqlDbType.Numeric, DbType.Decimal },
      { NpgsqlDbType.Double, DbType.Double },
      { NpgsqlDbType.Integer, DbType.Int32 },
      { NpgsqlDbType.Money, DbType.Currency },
      { NpgsqlDbType.Char, DbType.StringFixedLength },
      { NpgsqlDbType.Varchar, DbType.String },
      { NpgsqlDbType.Real, DbType.Single },
      { NpgsqlDbType.Integer, DbType.Int16 },
      { NpgsqlDbType.Time, DbType.Time },
      { NpgsqlDbType.Integer, DbType.Byte },
      { NpgsqlDbType.Uuid, DbType.Guid },
      //{ NpgsqlDbType.Binary, DbType.Binary },
      { NpgsqlDbType.Varchar, DbType.AnsiString },
      { NpgsqlDbType.Xml, DbType.Xml }
    };

    /// <summary>
    ///   Strong mapping of Sql Server Database type lower case names to their equivalent Enumeration.
    /// </summary>
    private static readonly Dictionary<string, NpgsqlDbType> SqlTypes = Utils.GetEnumDictionary<NpgsqlDbType>(true);
  }
}
