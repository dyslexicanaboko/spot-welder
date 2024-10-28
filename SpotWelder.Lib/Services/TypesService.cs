using SpotWelder.Lib.Services.TypeMappings;
using System;
using System.Collections.Generic;
using System.Data;

namespace SpotWelder.Lib.Services
{
  public class TypesService
  {
    //I got the base of this list from here: https://stackoverflow.com/a/1362899/603807
    //I am not using this right now because I realized later that the CodeProvider DOES provide the aliases. Going to keep it around for now.
    /// <summary>
    ///   Strong mapping of System types and their aliases. List has been extended to include
    ///   structures that are not primitive types but are used as such.
    /// </summary>
    public static readonly Dictionary<Type, string> MapSystemToAliases = new Dictionary<Type, string>
    {
      { typeof(byte), "byte" },
      { typeof(sbyte), "sbyte" },
      { typeof(short), "short" },
      { typeof(ushort), "ushort" },
      { typeof(int), "int" },
      { typeof(uint), "uint" },
      { typeof(long), "long" },
      { typeof(ulong), "ulong" },
      { typeof(float), "float" }, //Single
      { typeof(double), "double" },
      { typeof(decimal), "decimal" },
      { typeof(object), "object" },
      { typeof(bool), "bool" },
      { typeof(char), "char" },
      { typeof(string), "string" },
      { typeof(void), "void" },

      //These don't have aliases because they are not primitives, however they are used as such
      { typeof(TimeSpan), "TimeSpan" },
      { typeof(DateTime), "DateTime" },
      { typeof(DateTimeOffset), "DateTimeOffset" },
      { typeof(Guid), "Guid" },
    };
    
    /// <summary>
    ///  Mapping of System types to TypeScript types. This is a loose mapping and does not account for all types.
    /// </summary>
    /// <remarks>https://www.typescriptlang.org/docs/handbook/2/everyday-types.html</remarks>
    public static readonly Dictionary<Type, string> MapSystemToTypeScript = new Dictionary<Type, string>
    {
      { typeof(bool), "boolean" },
      { typeof(byte), "number" },
      { typeof(short), "number" },
      { typeof(int), "number" },
      { typeof(long), "bigint" },
      { typeof(string), "string" },
      { typeof(char[]), "string" },
      { typeof(byte[]), "Uint8Array" },
      { typeof(decimal), "number" },
      { typeof(float), "number" },
      { typeof(double), "number" },
      { typeof(TimeSpan), "string" },
      { typeof(DateTime), "Date" },
      { typeof(DateTimeOffset), "Date" },
      { typeof(Guid), "string" }
    };

    //TODO: I don't love that this isn't being controlled by Dependency Injection
    //I don't see a proper way to do this with DI that won't absolutely be a total waste of time for the sake of using DI
    public static TypeMappingBase GetTypeMapper(SqlEngine sqlEngine)
    {
      return sqlEngine switch
      {
        SqlEngine.SqlServer => new SqlServerTypeMapping(),
        SqlEngine.Postgres => new PostgresTypeMapping(),
        _ => throw new ArgumentOutOfRangeException(nameof(sqlEngine), sqlEngine, "SQL Engine not supported. 0x202411032339")
      };
    }
  }
}
