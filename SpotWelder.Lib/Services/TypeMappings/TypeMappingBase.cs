using System;
using System.Data;

namespace SpotWelder.Lib.Services.TypeMappings
{
  public abstract class TypeMappingBase
  {
    //Type -> "SqlDbType"
    /// <summary>
    /// Convert the <see cref="Type"/> to the SQL engine specific enumerated type as a string.
    /// </summary>
    /// <param name="type">System type</param>
    /// <returns>DbType</returns>
    public abstract string GetSqlDataTypeAsString(Type type);

    //DbType -> SqlDbType
    /// <summary>
    /// Convert the <see cref="DbType"/> to the SQL engine specific enumerated type.
    /// </summary>
    /// <param name="type">DbType enumeration</param>
    /// <returns>SQL engine specific type</returns>
    public abstract Enum GetDbTypeAsSqlDataTypeEnum(DbType type);

    //SqlDbType -> DbType
    /// <summary>
    /// Get the <see cref="DbType"/> from the SQL engine specific enumerated type.
    /// </summary>
    /// <param name="sqlDataType">SQL engine specific enumeration type</param>
    /// <returns>Loosely equivalent DbType</returns>
    public abstract DbType GetDbType(Enum sqlDataType);

    //"SqlDbType" -> DbType
    /// <summary>
    /// Get the <see cref="DbType"/> from the SQL engine specific enumerated type as a string.
    /// </summary>
    /// <param name="sqlDataTypeName">SQL engine specific enumeration type as a string</param>
    /// <returns>Loosely equivalent DbType</returns>
    public abstract DbType GetDbType(string sqlDataTypeName);
  }
}
