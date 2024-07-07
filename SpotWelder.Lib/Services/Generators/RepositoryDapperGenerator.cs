using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.CodeFactory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
	public class RepositoryDapperGenerator
		: GeneratorBase
	{
		public RepositoryDapperGenerator(ClassInstructions instructions)
			: base(instructions, "RepositoryDapper.cs.template")
		{
			instructions.ClassName = instructions.SubjectName;
		}

		public override GeneratedResult FillTemplate()
		{
			var strTemplate = GetTemplate(TemplateName);

			var template = new StringBuilder(strTemplate);

			template.Replace("{{Namespace}}", Instructions.Namespace);
			template.Replace("{{ClassName}}", Instructions.ClassName); //Prefix of the repository class
			template.Replace("{{EntityName}}", Instructions.EntityName);
			template.Replace("{{Namespaces}}", FormatNamespaces(Instructions.Namespaces));

			var t = template.ToString();

			t = RemoveExcessBlankSpace(t);
			//t = RemoveBlankLines(t);

			var pk = Instructions.Properties.SingleOrDefault(x => x.IsPrimaryKey);
			var lstNoPk = Instructions.Properties.Where(x => !x.IsPrimaryKey).ToList();
			var lstInsert = new List<ClassMemberStrings>(lstNoPk);

			//TODO: What to do when there is no primary key?
			if (pk != null)
			{
				t = t.Replace("{{PrimaryKeyParameter}}", pk.Parameter);  //taskId
				t = t.Replace("{{PrimaryKeyProperty}}", pk.Property);    //TaskId
				t = t.Replace("{{PrimaryKeyColumn}}", pk.ColumnName);    //TaskId or task_id
				t = t.Replace("{{PrimaryKeyType}}", pk.SystemTypeAlias); //int

				var scopeIdentity = string.Empty;

				if (pk.IsIdentity)
				{
					//If the PK is identity then the PK needs to be returned
					scopeIdentity = @"
			SELECT SCOPE_IDENTITY() AS PK;"; //Don't change the spacing here, it's like this on purpose
				}
				else
				{
					//If the PK is not identity, then the PK needs to explicitly be provided and inserted
					lstInsert.Insert(0, pk);
				}

				t = t.Replace("{{ScopeIdentity}}", scopeIdentity);
				t = t.Replace("{{PrimaryKeyInsertExecution}}", FormatInsertExecution(pk));
			}

			t = t.Replace("{{Schema}}", Instructions.TableQuery.Schema);
			t = t.Replace("{{Table}}", Instructions.TableQuery.Table);
			t = t.Replace("{{SelectAllList}}", FormatSelectList(Instructions.Properties));
			t = t.Replace("{{InsertColumnList}}", FormatSelectList(lstInsert));
			t = t.Replace("{{InsertValuesList}}", FormatSelectList(lstInsert, "@"));
			t = t.Replace("{{UpdateParameters}}", FormatUpdateList(lstNoPk));
			t = t.Replace("{{DynamicParametersInsert}}", FormatDynamicParameterList(lstInsert));
			t = t.Replace("{{DynamicParametersUpdate}}", FormatDynamicParameterList(Instructions.Properties));
			t = t.Replace("{{DynamicParametersDelete}}", FormatDynamicParameterList(new List<ClassMemberStrings> { pk }));

			return new GeneratedResult
			{
				Filename = $"{Instructions.ClassName}Repository.cs",
				Contents = t
			};
		}

		private string FormatSelectList(IList<ClassMemberStrings> properties, string prefix = null)
		{
			var content = GetTextBlock(properties,
				p => $"                {prefix}{p.Property}",
				"," + Environment.NewLine);

			return content;
		}

		private string FormatUpdateList(IList<ClassMemberStrings> properties)
		{
			var content = GetTextBlock(properties,
				p => $"                {p.Property} = @{p.Property}",
				"," + Environment.NewLine);

			return content;
		}

		private string FormatDynamicParameterList(IList<ClassMemberStrings> properties)
		{
			var content = GetTextBlock(properties, 
				p => $"{FormatDynamicParameter(p)}",
				Environment.NewLine);

			return content;
		}

		private string FormatDynamicParameter(ClassMemberStrings properties)
		{
			var t = properties.DatabaseType;

			var strDbType = TypesService.MapSqlDbTypeToDbTypeLoose.TryGetValue(t, out var dbType) ? 
				dbType.ToString() : 
				$"SqlDbType.{t}_MissingMapping";

			var lst = new List<string>
			{
				$"name: \"@{properties.Property}\"",
				$"dbType: DbType.{strDbType}",
				$"value: entity.{properties.Property}"
			};

			//TODO: Need to work through every type to see what the combinations are
			switch (t)
			{
				case SqlDbType.DateTime2:
					lst.Add($"scale: {properties.Scale}");
					break;
				case SqlDbType.Decimal:
					lst.Add($"precision: {properties.Precision}, scale: {properties.Scale}");
					break;
				case SqlDbType.VarChar:
				case SqlDbType.NVarChar:
				case SqlDbType.Char:
				case SqlDbType.NChar:
					lst.Add($"size: {properties.Size}");
					break;
			}

			var content = $"				p.Add({string.Join(", ", lst)});";

			return content;
		}

		private string FormatInsertExecution(ClassMemberStrings primaryKey)
		{
			string content;

			if (primaryKey.IsIdentity)
			{
				content = $"            return connection.ExecuteScalar<{primaryKey.SystemTypeAlias}>(sql, entity);";
			}
			else
			{
				content = 
$@"            connection.Execute(sql, p);

				return entity.{primaryKey.Property};";
			}

			return content;
		}
	}
}