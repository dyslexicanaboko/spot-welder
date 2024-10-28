using System;
using System.Data;

namespace SpotWelder.Lib.Services.TypeMappings
{
  public abstract class TypeMappingBase
  {
    //Type -> "SqlDbType"
    public abstract string GetSqlDataTypeAsString(Type type);

    //SqlDbType -> DbType
    public abstract DbType GetDbTypeBySqlDataType(Enum sqlDataType);

    //"SqlDbType" -> SqlDbType
    public abstract Enum GetSqlDataType(string sqlDataTypeName);
  }
}
