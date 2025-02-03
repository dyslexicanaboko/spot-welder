using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.CodeFactory;
using SpotWelder.Lib.Services.Generators.SqlEngineStrategies;
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
			var syntax = BaseSqlEngineSyntax.GetSyntax(instructions.SqlEngine);

			instructions.ClassName = instructions.SubjectName;

			var strTemplate = GetTemplate(TemplateName);

			var template = new StringBuilder(strTemplate);

			template.Replace("{{Namespace}}", instructions.Namespace);
			template.Replace("{{ClassName}}", instructions.ClassName); //Prefix of the repository class
			template.Replace("{{EntityName}}", instructions.EntityName);
			template.Replace("{{Namespaces}}", FormatNamespaces(instructions.Namespaces));
			template.Replace("{{SqlNamespaces}}", FormatNamespaces(syntax.SqlNamespaces));
			template.Replace("{{ConnectionObject}}", syntax.ConnectionObject);
			template.Replace("{{ParameterObject}}", syntax.ParameterObject);
			template.Replace("{{ParameterDbTypeProperty}}", syntax.ParameterDbTypeProperty);
			template.Replace("{{ParameterDbTypeEnum}}", syntax.ParameterDbTypeEnum);

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
				template.Replace("{{PrimaryKeyDbType}}", pk.DatabaseType.ToString()); //DbType.Int32

				var scopeIdentity = ScopeIdentityValues.Empty();

				if (pk.IsIdentity)
					//If the PK is identity then the PK needs to be returned
					scopeIdentity = syntax.GetScopeIdentity(pk.ColumnName);
				else
					//If the PK is not identity, then the PK needs to explicitly be provided and inserted
					lstInsert.Insert(0, pk);

				template.Replace("{{InsertPkColumnName}}", scopeIdentity.PrimaryKeyColumnName);
				template.Replace("{{InsertPkDefault}}", scopeIdentity.PrimaryKeyDefault);
				template.Replace("{{ScopeIdentity}}", scopeIdentity.ScopeIdentity);
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
			
			return GetFormattedCSharpResult($"{instructions.ClassName}Repository.cs", template);
		}

		private string FormatSelectList(IList<ClassMemberStrings> properties, string? prefix = null)
		{
			var content = GetTextBlock(
				properties,
				p => $"                {prefix}{p.ColumnName}",
				"," + Environment.NewLine);

			return content;
		}

		private string FormatUpdateList(IList<ClassMemberStrings> properties)
		{
			var content = GetTextBlock(
				properties,
				p => $"                {p.ColumnName} = @{p.ColumnName}",
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
			var lst = new List<string>
			{
				$"name: \"@{properties.ColumnName}\"", $"dbType: DbType.{properties.DatabaseType}", $"value: entity.{properties.Property}"
			};

			//TODO: Need to work through every type to see what the combinations are
			switch (properties.DatabaseType)
			{
				case DbType.DateTime2:
					lst.Add($"scale: {properties.Scale}");

					break;

				case DbType.Decimal:
					lst.Add($"precision: {properties.Precision}, scale: {properties.Scale}");

					break;

				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
				case DbType.String:
				case DbType.StringFixedLength:
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

			return primaryKey.IsIdentity ? 
				$"            return {ak}connection.ExecuteScalar{suf}<{primaryKey.SystemTypeAlias}>(sql, p);" : 
				$@"            {ak}connection.Execute{suf}(sql, p);

				return entity.{primaryKey.Property};";
		}
	}
}
