using SpotWelder.Lib.Exceptions;
using SpotWelder.Lib.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SpotWelder.Lib.DataAccess
{
  /// <summary>
  ///   The Data Access layer for the Code Generator
  /// </summary>
  public class QueryToClassRepository
    : BaseRepository, IQueryToClassRepository
  {
    public SchemaQuery GetSchema(TableQuery tableQuery, SourceSqlType sourceSqlType, string sourceSqlText)
    {
      var query = SqlClient.GetSchemaQuery(sourceSqlType, sourceSqlText);

      var rs = GetFullSchemaInformation(query);

      var sq = new SchemaQuery(SqlClient.SqlEngine, tableQuery, query);
      
      sq.ColumnsAll = new List<SchemaColumn>(rs.GenericSchema.Columns.Count);

      foreach (DataColumn dc in rs.GenericSchema.Columns)
      {
        var arr = rs.SqlServerSchema.Select($"ColumnName = '{dc.ColumnName}'");

        //If the query provided contains repeat column names, this won't work. Let the user know they have to change their query.
        if (arr.Length > 1) throw new NonUniqueColumnException(dc.ColumnName);

        var sqlServerColumn = arr.Single();

        var sc = new SchemaColumn(
          sq.SourceSqlEngine,
          dc.ColumnName,
          dc.DataType,
          sqlServerColumn.Field<string>("DataTypeName"),
          false, //Cannot be determined from DataColumn type directly
          dc.AutoIncrement,
          dc.AllowDBNull,
          sqlServerColumn.Field<int>("ColumnSize"),

          //These two fields are int16 for SQL Server, but are int32 for Postgres. Using int32 for both.
          sqlServerColumn.Field<int>("NumericPrecision"),
          sqlServerColumn.Field<int>("NumericScale")
        );

        sq.ColumnsAll.Add(sc);
      }

      //Check if any primary keys exist
      if (!rs.GenericSchema.PrimaryKey.Any()) return sq;

      //TODO: This is assuming a single column is the primary key which is a bad idea, but okay for now
      var pk = rs.GenericSchema.PrimaryKey.First();

      sq.SetPrimaryKey(sq
        .ColumnsAll
        .Single(x => x.ColumnName.Equals(pk.ColumnName, StringComparison.InvariantCultureIgnoreCase)));

      sq.PrimaryKey!.IsIdentity = pk.AutoIncrement;
      sq.PrimaryKey.IsPrimaryKey = true;

      sq.ColumnsNoPk = sq
        .ColumnsAll
        .Where(x => !x.ColumnName.Equals(pk.ColumnName, StringComparison.InvariantCultureIgnoreCase))
        .ToList();

      return sq;
    }
  }
}
