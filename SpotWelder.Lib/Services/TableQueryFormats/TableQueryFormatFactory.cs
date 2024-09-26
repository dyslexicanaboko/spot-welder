using System.Collections.Generic;
using System.Linq;

namespace SpotWelder.Lib.Services.TableQueryFormats
{
  public class TableQueryFormatFactory
    : ITableQueryFormatFactory
  {
    private readonly Dictionary<SqlEngine, ITableQueryFormatStrategy> _strategies;

    public TableQueryFormatFactory(IEnumerable<ITableQueryFormatStrategy> strategies)
    {
      _strategies = strategies.ToDictionary(k => k.SqlEngine, v => v);
    }

    public ITableQueryFormatStrategy GetStrategy(SqlEngine sqlEngine)
    {
      if (!_strategies.ContainsKey(sqlEngine))
        throw new KeyNotFoundException($"No strategy found for {sqlEngine}");

      return _strategies[sqlEngine];
    }
  }
}
