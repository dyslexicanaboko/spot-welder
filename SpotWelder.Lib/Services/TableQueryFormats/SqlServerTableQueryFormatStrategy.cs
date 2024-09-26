using System;
using System.Collections.Generic;

namespace SpotWelder.Lib.Services.TableQueryFormats
{
  public class SqlServerTableQueryFormatStrategy
    : BaseTableQueryFormatStrategy, ITableQueryFormatStrategy
  {
    public override SqlEngine SqlEngine => SqlEngine.SqlServer;

    protected override string DefaultSchema => "dbo";

    protected override string RemoveQualifiers(string tableNameQuery) => tableNameQuery
      .Replace("[", string.Empty)
      .Replace("]", string.Empty);

    protected override void Qualify(ICollection<string> segments, TableQueryQualifiers qualifier, string segment)
    {
      if (string.IsNullOrWhiteSpace(segment)) throw new ArgumentException($"{qualifier} cannot be null or whitespace.");

      var qualified = $"[{segment}]";

      segments.Add(qualified);
    }
  }
}
