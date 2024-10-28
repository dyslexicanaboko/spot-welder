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
		public override GenerationElections Election => GenerationElections.RepoDapper;

		protected override string TemplateName => "RepositoryDapper.cs.template";

		public override GeneratedResult FillTemplate(ClassInstructions instructions)
		{
			instructions.ClassName = instructions.SubjectName;

			var strTemplate = GetTemplate(TemplateName);

			var template = new StringBuilder(strTemplate);

			template.Replace("{{Namespace}}", instructions.Namespace);
			template.Replace("{{ClassName}}", instructions.ClassName); //Prefix of the repository class
			template.Replace("{{EntityName}}", instructions.EntityName);
			template.Replace("{{Namespaces}}", FormatNamespaces(instructions.Namespaces));

			GetAsynchronicityFormatStrategy(instructions.IsAsynchronous).ReplaceTags(template);

			var pk = instructions.Properties.SingleOrDefault(x => x.IsPrimaryKey);
			var lstNoPk = instructions.Properties.Where(x => !x.IsPrimaryKey).ToList();
			var lstInsert = new List<ClassMemberStrings>(lstNoPk);

			//TODO: What to do when there is no primary key?
			if (pk != null)
			{
				template.Replace("{{PrimaryKeyParameter}}", pk.Parameter); //taskId
				template.Replace("{{PrimaryKeyProperty}}", pk.Property); //TaskId
				template.Replace("{{PrimaryKeyColumn}}", pk.ColumnName); //TaskId or task_id
				template.Replace("{{PrimaryKeyType}}", pk.SystemTypeAlias); //int

				var scopeIdentity = string.Empty;

				if (pk.IsIdentity)

					//If the PK is identity then the PK needs to be returned
					scopeIdentity = @"
			SELECT SCOPE_IDENTITY() AS PK;"; //Don't change the spacing here, it's like this on purpose
				else

					//If the PK is not identity, then the PK needs to explicitly be provided and inserted
					lstInsert.Insert(0, pk);

				template.Replace("{{ScopeIdentity}}", scopeIdentity);
				template.Replace("{{PrimaryKeyInsertExecution}}", FormatInsertExecution(pk, instructions.IsAsynchronous));
			}

			template.Replace("{{Schema}}", instructions.TableQuery.Schema);
			template.Replace("{{Table}}", instructions.TableQuery.Table);
			template.Replace("{{SelectAllList}}", FormatSelectList(instructions.Properties));
			template.Replace("{{InsertColumnList}}", FormatSelectList(lstInsert));
			template.Replace("{{InsertValuesList}}", FormatSelectList(lstInsert, "@"));
			template.Replace("{{UpdateParameters}}", FormatUpdateList(lstNoPk));
			template.Replace("{{DynamicParametersInsert}}", FormatDynamicParameterList(lstInsert));
			template.Replace("{{DynamicParametersUpdate}}", FormatDynamicParameterList(instructions.Properties));
			template.Replace("{{DynamicParametersDelete}}", FormatDynamicParameterList(new List<ClassMemberStrings> { pk }));

			return GetFormattedCSharpResult($"{instructions.ClassName}Repository.cs", template);
		}

		private string FormatSelectList(IList<ClassMemberStrings> properties, string prefix = null)
		{
			var content = GetTextBlock(
				properties,
				p => $"                {prefix}{p.Property}",
				"," + Environment.NewLine);

			return content;
		}

		private string FormatUpdateList(IList<ClassMemberStrings> properties)
		{
			var content = GetTextBlock(
				properties,
				p => $"                {p.Property} = @{p.Property}",
				"," + Environment.NewLine);

			return content;
		}

		private string FormatDynamicParameterList(IList<ClassMemberStrings> properties)
		{
			var content = GetTextBlock(
				properties,
				p => $"{FormatDynamicParameter(p)}",
				Environment.NewLine);

			return content;
		}

		private static string FormatDynamicParameter(ClassMemberStrings properties)
		{
			var t = properties.DatabaseType;

			var strDbType = TypesService.MapSqlDbTypeToDbTypeLoose.TryGetValue(t, out var dbType) ?
				dbType.ToString() :
				$"[{t}]_MissingMapping";

			var lst = new List<string>
			{
				$"name: \"@{properties.Property}\"", $"dbType: DbType.{strDbType}", $"value: entity.{properties.Property}"
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

		private static string FormatInsertExecution(ClassMemberStrings primaryKey, bool isAsynchronous)
		{
			var ak = string.Empty;
			var suf = string.Empty;

			if (isAsynchronous)
			{
				ak = "await ";
				suf = "Async";
			}

			if (primaryKey.IsIdentity)
				return $"            return {ak}connection.ExecuteScalar{suf}<{primaryKey.SystemTypeAlias}>(sql, entity);";

			return
				$@"            {ak}connection.Execute{suf}(sql, p);

				return entity.{primaryKey.Property};";
		}
	}
}
