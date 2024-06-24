using Microsoft.Data.SqlClient;
using System;
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

			var p = GetPrimaryKeyParameter({{PrimaryKeyParameter}});

			using var dr = ExecuteReaderText(sql, p);
			
			var lst = ToList(dr, ToEntity);

			return !lst.Any() ? return null : lst.Single();
		}

		public IEnumerable<{{EntityName}}> SelectAll()
		{
			const string sql = @"
			SELECT
{{SelectAllList}}
			FROM {{Schema}}.{{Table}}";

			using var dr = ExecuteReaderText(sql);
			
			return ToList(dr, ToEntity);
		}

		//Preference on whether or not insert method returns a value is up to the user and the object being inserted
		public {{PrimaryKeyType}} Insert({{EntityName}} entity)
		{
			const string sql = @"INSERT INTO {{Schema}}.{{Table}} (
{{InsertColumnList}}
						) VALUES (
{{InsertValuesList}});
{{ScopeIdentity}}";

			var lst = GetParameters(entity);

{{PrimaryKeyInsertExecution}}
		}

		public void Update({{EntityName}} entity)
		{
			const string sql = @"UPDATE {{Schema}}.{{Table}} SET 
{{UpdateParameters}}
					WHERE {{PrimaryKeyColumn}} = @{{PrimaryKeyProperty}}";

			var lst = GetParameters(entity);

			var p = GetPrimaryKeyParameter(entity.{{PrimaryKeyProperty}});

			lst.Add(p);

			ExecuteNonQuery(sql, lst.ToArray());
		}

		public void Delete({{EntityName}} entity)
		{
			const string sql = @"DELETE FROM {{Schema}}.{{Table}} WHERE {{PrimaryKeyColumn}} = @{{PrimaryKeyProperty}}";

			var arr = new SqlParameter[] { GetPrimaryKeyParameter(entity.{{PrimaryKeyProperty}}) };
			
			ExecuteNonQuery(sql, arr);
		}
		
		private SqlParameter GetPrimaryKeyParameter({{PrimaryKeyType}} {{PrimaryKeyParameter}})
		{
			var p = new SqlParameter();
			p.ParameterName = "@{{PrimaryKeyProperty}}";
			p.SqlDbType = SqlDbType.{{PrimaryKeySqlDbType}};
			p.Value = {{PrimaryKeyParameter}};
			
			return p;
		}

		private List<SqlParameter> GetParameters({{EntityName}} entity)
		{
			SqlParameter p = null;

			var lst = new List<SqlParameter>();
			
{{SqlParameters}}
			
			return lst;
		}
		
		private {{EntityName}} ToEntity(IDataReader reader)
		{
			var r = reader;

			var e = new {{EntityName}}();
{{SetProperties}}

			return e;
		}
	}
}
