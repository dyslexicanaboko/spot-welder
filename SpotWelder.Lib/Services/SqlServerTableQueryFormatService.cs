using System;
using System.Collections.Generic;

namespace SpotWelder.Lib.Services
{
  public class SqlServerTableQueryFormatService
    : BaseTableQueryFormatService, ITableQueryFormatService
  {
    protected override string DefaultSchema => "dbo";

    protected override string RemoveQualifiers(string tableNameQuery)
    {
      return tableNameQuery
        .Replace("[", string.Empty)
        .Replace("]", string.Empty);
    }
    
    protected override void Qualify(ICollection<string> segments, TableQueryQualifiers qualifier, string segment)
    {
      if (string.IsNullOrWhiteSpace(segment)) throw new ArgumentException($"{qualifier} cannot be null or whitespace.");

      var qualified = $"[{segment}]";

      segments.Add(qualified);
    }
  }
}
