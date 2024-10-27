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

      var sq = new SchemaQuery();
      sq.Query = query;
      sq.TableQuery = tableQuery; //TODO: Should probably clone this object to be safe?
      sq.IsSolitaryTableQuery = sq.TableQuery != null;
      sq.HasPrimaryKey = rs.GenericSchema.PrimaryKey.Any();

      sq.ColumnsAll = new List<SchemaColumn>(rs.GenericSchema.Columns.Count);

      foreach (DataColumn dc in rs.GenericSchema.Columns)
      {
        var arr = rs.SqlServerSchema.Select($"ColumnName = '{dc.ColumnName}'");

        //If the query provided contains repeat column names, this won't work. Let the user know they have to change their query.
        if (arr.Length > 1) throw new NonUniqueColumnException(dc.ColumnName);

        var sqlServerColumn = arr.Single();

        var sc = new SchemaColumn
        {
          ColumnName = dc.ColumnName,
          IsDbNullable = dc.AllowDBNull,
          SystemType = dc.DataType,
          SqlType = sqlServerColumn.Field<string>("DataTypeName"),
          Size = sqlServerColumn.Field<int>("ColumnSize"),
          //These two fields are int16 for SQL Server, but are int32 for Postgres. Using int32 for both.
          Precision = sqlServerColumn.Field<int>("NumericPrecision"),
          Scale = sqlServerColumn.Field<int>("NumericScale")
        };

        sq.ColumnsAll.Add(sc);
      }

      if (!sq.HasPrimaryKey) return sq;

      //TODO: This is assuming a single column is the primary key which is a bad idea, but okay for now
      var pk = rs.GenericSchema.PrimaryKey.First();

      sq.PrimaryKey = sq
        .ColumnsAll
        .Single(x => x.ColumnName.Equals(pk.ColumnName, StringComparison.InvariantCultureIgnoreCase));

      sq.PrimaryKey.IsIdentity = pk.AutoIncrement;
      sq.PrimaryKey.IsPrimaryKey = true;

      sq.ColumnsNoPk = sq
        .ColumnsAll
        .Where(x => !x.ColumnName.Equals(pk.ColumnName, StringComparison.InvariantCultureIgnoreCase))
        .ToList();

      return sq;
    }
  }
}
