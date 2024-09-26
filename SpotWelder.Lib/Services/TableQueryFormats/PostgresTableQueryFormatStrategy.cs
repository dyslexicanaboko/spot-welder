using System;
using System.Collections.Generic;
using System.Linq;

namespace SpotWelder.Lib.Services.TableQueryFormats
{
  public class PostgresTableQueryFormatStrategy
    : BaseTableQueryFormatStrategy, ITableQueryFormatStrategy
  {
    public override SqlEngine SqlEngine => SqlEngine.Postgres;

    protected override string DefaultSchema => "public";

    protected override string RemoveQualifiers(string tableNameQuery) => tableNameQuery.Replace("\"", string.Empty);

    protected override void Qualify(ICollection<string> segments, TableQueryQualifiers qualifier, string segment)
    {
      if (string.IsNullOrWhiteSpace(segment)) throw new ArgumentException($"{qualifier} cannot be null or whitespace.");

      var qualified = segment;

      //If there is any whitespace or an uppercase letter, qualify the segment
      if (_whiteSpace.IsMatch(segment) || ContainsUpperCase(segment)) qualified = $"\"{segment}\"";

      segments.Add(qualified);

      bool ContainsUpperCase(string s) => s.Any(char.IsUpper);
    }
  }
}
