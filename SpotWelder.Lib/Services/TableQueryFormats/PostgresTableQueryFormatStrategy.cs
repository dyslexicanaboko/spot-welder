using System;
using System.Collections.Generic;

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

      // One less thing to worry about, just make it lowercase
      var qualified = segment.ToLowerInvariant();

      //NOTE: the strict rules of postgres are if there is any whitespace or an uppercase letter, qualify the segment
      // To not drive anyone crazy, I will just be lowercasing everything automatically.

      //If there is any whitespace or an uppercase letter, qualify the segment
      if (WhiteSpace.IsMatch(segment)) qualified = $"\"{segment}\"";

      segments.Add(qualified);

      //return;

      //static bool ContainsUpperCase(string s) => s.Any(char.IsUpper);
    }
  }
}
