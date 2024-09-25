using System;
using System.Collections.Generic;
using System.Linq;

namespace SpotWelder.Lib.Services
{
  public class PostgresTableQueryFormatService
    : BaseTableQueryFormatService, ITableQueryFormatService
  {
    protected override string DefaultSchema => "public";

    protected override string RemoveQualifiers(string tableNameQuery)
    {
      return tableNameQuery.Replace("\"", string.Empty);
    }
    
    protected override void Qualify(ICollection<string> segments, TableQueryQualifiers qualifier, string segment)
    {
      if (string.IsNullOrWhiteSpace(segment)) throw new ArgumentException($"{qualifier} cannot be null or whitespace.");

      var qualified = segment;

      //If there is any whitespace or an uppercase letter, qualify the segment
      if (_whiteSpace.IsMatch(segment) || ContainsUpperCase(segment))
      {
        qualified = $"\"{segment}\"";
      }

      segments.Add(qualified);

      bool ContainsUpperCase(string s) => s.Any(char.IsUpper);
    }
  }
}
