using SpotWelder.Lib.Models;

namespace SpotWelder.Lib.DataAccess
{
  public interface IQueryToClassRepository
    : IBaseRepository
  {
    SchemaQuery GetSchema(TableQuery tableQuery, SourceSqlType sourceSqlType, string sourceSqlText);
  }
}
