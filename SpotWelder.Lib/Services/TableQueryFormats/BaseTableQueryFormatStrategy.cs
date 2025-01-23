using Humanizer;
using SpotWelder.Lib.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SpotWelder.Lib.Services.TableQueryFormats
{
  public abstract class BaseTableQueryFormatStrategy
  {
    protected readonly Regex _whiteSpace = new(@"\s+");

    public abstract SqlEngine SqlEngine { get; }

    protected abstract string DefaultSchema { get; }

    public virtual string GetClassName(TableQuery tableQuery)
      => _whiteSpace.Replace(tableQuery.TableUnqualified, string.Empty).Dehumanize();

    protected abstract string RemoveQualifiers(string tableNameQuery);

    protected abstract void Qualify(ICollection<string> segments, TableQueryQualifiers qualifier, string segment);

    public virtual TableQuery ParseTableName(string tableNameQuery)
    {
      var q = RemoveQualifiers(tableNameQuery);

      var arr = q.Split('.');

      var tbl = new TableQuery();

      switch (arr.Length)
      {
        //Table
        case 1:
          tbl.Schema = DefaultSchema;
          tbl.Table = arr[0];

          break;

        //Schema.Table
        case 2:
          tbl.Schema = arr[0];
          tbl.Table = arr[1];

          break;

        //Database.Schema.Table
        case 3:
          tbl.Database = arr[0];
          tbl.Schema = arr[1];
          tbl.Table = arr[2];

          break;

        //LinkedServer.Database.Schema.Table
        case 4:
          tbl.LinkedServer = arr[0];
          tbl.Database = arr[1];
          tbl.Schema = arr[2];
          tbl.Table = arr[3];

          break;
      }

      //Copy the unqualified version before it is qualified
      tbl.TableUnqualified = tbl.Table;

      return tbl;
    }

    public virtual string FormatTableQuery(
      TableQuery tableQuery,
      TableQueryQualifiers qualifiers = TableQueryQualifiers.Schema | TableQueryQualifiers.Table)
    {
      if (qualifiers == TableQueryQualifiers.None)
        throw new ArgumentException("Qualifier cannot be none.", nameof(qualifiers));

      var lst = new List<string>();

      if (qualifiers.HasFlag(TableQueryQualifiers.LinkedServer))
        Qualify(lst, TableQueryQualifiers.LinkedServer, tableQuery.LinkedServer);

      if (qualifiers.HasFlag(TableQueryQualifiers.Database))
        Qualify(lst, TableQueryQualifiers.Database, tableQuery.Database);

      if (qualifiers.HasFlag(TableQueryQualifiers.Schema))
      {
        var schema = tableQuery.Schema;

        if (string.IsNullOrWhiteSpace(schema)) schema = DefaultSchema;

        Qualify(lst, TableQueryQualifiers.Schema, schema);
      }

      if (qualifiers.HasFlag(TableQueryQualifiers.Table)) Qualify(lst, TableQueryQualifiers.Table, tableQuery.Table);

      var strTableQuery = string.Join(".", lst);

      return strTableQuery;
    }

    public virtual string FormatTableQuery(
      string tableQuery,
      TableQueryQualifiers qualifiers = TableQueryQualifiers.Schema | TableQueryQualifiers.Table)
    {
      var tq = ParseTableName(tableQuery);

      var str = FormatTableQuery(tq, qualifiers);

      return str;
    }
  }
}
