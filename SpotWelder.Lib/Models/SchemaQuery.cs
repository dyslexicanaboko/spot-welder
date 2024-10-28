using System.Collections.Generic;

namespace SpotWelder.Lib.Models
{
  public class SchemaQuery
  {
    public SchemaQuery(
      SqlEngine sourceSqlEngine,
      TableQuery tableQuery,
      string query)
    {
      SourceSqlEngine = sourceSqlEngine;
      TableQuery = tableQuery; //TODO: Should probably clone this object to be safe?
      Query = query;

      IsSolitaryTableQuery = TableQuery != null; //When can this really be null?
    }

    /// <summary> Some parts of the code generation are treated differently depending on the SQL Engine. </summary>
    public SqlEngine SourceSqlEngine { get; }

    /// <summary> The source query that produced this schema query object. </summary>
    public string Query { get; }

    /// <summary>
    ///   Denotes whether or not the original query is for a solitary table or if the query
    ///   involves more than one table.
    /// </summary>
    public bool IsSolitaryTableQuery { get; }

    public TableQuery TableQuery { get; }

    public bool HasPrimaryKey { get; private set; }

    public SchemaColumn? PrimaryKey { get; private set; }

    public IList<SchemaColumn> ColumnsAll { get; set; } = new List<SchemaColumn>();

    public IList<SchemaColumn> ColumnsNoPk { get; set; } = new List<SchemaColumn>();

    public void SetPrimaryKey(SchemaColumn schemaColumn)
    {
      PrimaryKey = schemaColumn;
      
      HasPrimaryKey = true;
    }
  }
}
