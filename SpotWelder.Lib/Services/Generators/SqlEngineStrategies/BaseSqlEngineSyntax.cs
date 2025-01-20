using SpotWelder.Lib.Services.CodeFactory;
using SpotWelder.Lib.Services.TypeMappings;
using System;

namespace SpotWelder.Lib.Services.Generators.SqlEngineStrategies
{
  public abstract class BaseSqlEngineSyntax
  {
    protected readonly TypeMappingBase SqlEngineTypes;

    protected BaseSqlEngineSyntax(SqlEngine sqlEngine)
    {
      SqlEngineTypes = TypesService.GetTypeMapper(sqlEngine);
    }

    /// <summary>
    /// The generic DBType cannot be used in this situation because it's SQL Engine specific.
    /// This method will resolve the appropriate syntax for the SQL Engine.
    /// </summary>
    /// <param name="properties"></param>
    /// <returns></returns>
    public abstract string FormatSqlParameter(ClassMemberStrings properties);

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
