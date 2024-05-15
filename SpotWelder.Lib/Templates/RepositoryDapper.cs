//https://www.nuget.org/packages/Dapper/
using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
{{Namespaces}}

namespace {{Namespace}}
{
    public class {{ClassName}}Repository
    {
		public {{EntityName}} Select({{PrimaryKeyType}} {{PrimaryKeyParameter}})
		{
      const string sql = @"
			SELECT
{{SelectAllList}}
			FROM {{Schema}}.{{Table}}
			WHERE {{PrimaryKeyColumn}} = @{{PrimaryKeyProperty}}";

      using var connection = new SqlConnection(ConnectionString);
      
      var lst = connection.Query<{{EntityName}}>(sql, new { {{PrimaryKeyProperty}} = {{PrimaryKeyParameter}}}).ToList();
        
      return !lst.Any()? null : lst.Single();
		}

		public IEnumerable<{{EntityName}}> SelectAll()
		{
      const string sql = @"
			SELECT
{{SelectAllList}}
			FROM {{Schema}}.{{Table}}";

      using var connection = new SqlConnection(ConnectionString);
      
      return connection.Query<{{EntityName}}>(sql).ToList();
		}

		//Preference on whether or not insert method returns a value is up to the user and the object being inserted
		public {{PrimaryKeyType}} Insert({{EntityName}} entity)
		{
      const string sql = @"INSERT INTO {{Schema}}.{{Table}} (
{{InsertColumnList}}
            ) VALUES (
{{InsertValuesList}});
{{ScopeIdentity}}";

      using var connection = new SqlConnection(ConnectionString);

      var p = new DynamicParameters();
{{DynamicParametersInsert}}
				
{{PrimaryKeyInsertExecution}}
		}

		public void Update({{EntityName}} entity)
		{
      const string sql = @"UPDATE {{Schema}}.{{Table}} SET 
{{UpdateParameters}}
            WHERE {{PrimaryKeyColumn}} = @{{PrimaryKeyProperty}}";

      using var connection = new SqlConnection(ConnectionString);

			var p = new DynamicParameters();
{{DynamicParametersUpdate}}

			connection.Execute(sql, p);
		}

    public void Delete({{EntityName}} entity)
    {
      const string sql = "DELETE FROM {{Schema}}.{{Table}} WHERE {{PrimaryKeyColumn}} = @{{PrimaryKeyProperty}}";

      using var connection = new SqlConnection(ConnectionString);

      var p = new DynamicParameters();
{{DynamicParametersUpdate}}

      connection.Execute(sql, p);
    }
	}
}
