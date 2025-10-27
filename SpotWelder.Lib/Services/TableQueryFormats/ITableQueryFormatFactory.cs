namespace SpotWelder.Lib.Services.TableQueryFormats;

public interface ITableQueryFormatFactory
{
  ITableQueryFormatStrategy GetStrategy(SqlEngine sqlEngine);
}
