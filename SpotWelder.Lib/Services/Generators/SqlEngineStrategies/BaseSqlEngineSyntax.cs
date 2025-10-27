using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.CodeFactory;
using SpotWelder.Lib.Services.TypeMappings;
using System;
using System.Collections.Generic;
using System.Data;

namespace SpotWelder.Lib.Services.Generators.SqlEngineStrategies
{
  public abstract class BaseSqlEngineSyntax
  {
    protected readonly TypeMappingBase SqlEngineTypes;

    protected BaseSqlEngineSyntax(SqlEngine sqlEngine)
    {
      SqlEngineTypes = TypesService.GetTypeMapper(sqlEngine);
    }

    /// <summary> The Sql Engine's SDK namespaces. </summary>
    public abstract IList<string> SqlNamespaces { get; protected set; }

    /// <summary> The Sql Engine's SDK connection object name. </summary>
    public abstract string ConnectionObject { get; protected set; }

    /// <summary> The Sql Engine's SDK parameter object name. </summary>
    public abstract string ParameterObject { get; protected set; }

    /// <summary> The Sql Engine's SDK parameter object's DB Type property name. </summary>
    public abstract string ParameterDbTypeProperty { get; protected set; }

    /// <summary> The Sql Engine's SDK parameter object's DB Type enumeration name. </summary>
    public abstract string ParameterDbTypeEnum { get; protected set; }

    /// <summary> The Sql Engine's sql syntax for returning the ID of the row that was just inserted. </summary>
    public abstract ScopeIdentityValues GetScopeIdentity(string pkColumnName);
    
    /// <summary>
    /// The generic DBType cannot be used in this situation because it's SQL Engine specific.
    /// This method will resolve the appropriate syntax for the SQL Engine.
    /// </summary>
    /// <param name="properties"></param>
    /// <returns></returns>
    public abstract string FormatSqlParameter(ClassMemberStrings properties);

    public virtual string GetEngineSpecificType(DbType type)
      => SqlEngineTypes.GetDbTypeAsSqlDataTypeEnum(type).ToString();

    public static BaseSqlEngineSyntax GetSyntax(SqlEngine sqlEngine)
    {
      return sqlEngine switch
      {
        SqlEngine.SqlServer => new SqlServerSyntax(sqlEngine),
        SqlEngine.Postgres => new PostgresSyntax(sqlEngine),
        _ => throw new ArgumentOutOfRangeException(nameof(sqlEngine), sqlEngine, "SQL Engine not supported. 0x202411032339")
      };
    }
  }
}
