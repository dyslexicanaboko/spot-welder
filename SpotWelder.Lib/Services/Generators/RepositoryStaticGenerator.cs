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
	public class RepositoryStaticGenerator
		: GeneratorBase
	{
		public override GenerationElections Election => GenerationElections.RepoStatic;

		protected override string TemplateName => "RepositoryStatic.cs.template";

		public override GeneratedResult FillTemplate(ClassInstructions instructions)
		{
			var syntax = BaseSqlEngineSyntax.GetSyntax(instructions.SqlEngine);

			var strTemplate = GetTemplate(TemplateName);

			var template = new StringBuilder(strTemplate);

			/* Context
			 * ClassName: Refers to the name of THIS class that is being generated "Table1Repository.cs"
			 * EntityName: Refers to the existing source Entity Class assumed to have been generated already "Table1Entity.cs"
			 * ModelName: Refers to the existing Model Class that compliments the Entity Class "Table1Model.cs" */

			template.Replace("{{Namespace}}", instructions.Namespace);
			template.Replace("{{ClassName}}", instructions.SubjectName); //Prefix of the repository class name
			template.Replace("{{EntityName}}", instructions.EntityName); //Class entity name
			template.Replace("{{Namespaces}}", FormatNamespaces(instructions.Namespaces));
			
			var pk = instructions.Properties.SingleOrDefault(x => x.IsPrimaryKey);
			var lstNoPk = instructions.Properties.Where(x => !x.IsPrimaryKey).ToList();
			var lstInsert = new List<ClassMemberStrings>(lstNoPk);

			//TODO: What to do when there is no primary key?
			if (pk != null)
			{
				template.Replace("{{PrimaryKeyParameter}}", pk.Parameter);
				template.Replace("{{PrimaryKeyProperty}}", pk.Property);
				template.Replace("{{PrimaryKeyColumn}}", pk.ColumnName);
				template.Replace("{{PrimaryKeyType}}", pk.SystemTypeAlias);
				template.Replace("{{PrimaryKeySqlDbType}}", pk.DatabaseType.ToString());

				var scopeIdentity = string.Empty;

				if (pk.IsIdentity)

					//If the PK is identity then the PK needs to be returned
					scopeIdentity = @"
			SELECT SCOPE_IDENTITY() AS PK;";
				else

					//If the PK is not identity, then the PK needs to explicitly be provided and inserted
					lstInsert.Insert(0, pk);

				template.Replace("{{ScopeIdentity}}", scopeIdentity);
				template.Replace("{{PrimaryKeyInsertExecution}}", FormatInsertExecution(pk));
			}

			template.Replace("{{Schema}}", instructions.TableQuery.Schema);
			template.Replace("{{Table}}", instructions.TableQuery.Table);
			template.Replace("{{SelectAllList}}", FormatSelectList(instructions.Properties));
			template.Replace("{{InsertColumnList}}", FormatSelectList(lstInsert));
			template.Replace("{{InsertValuesList}}", FormatSelectList(lstInsert, "@"));
			template.Replace("{{UpdateParameters}}", FormatUpdateList(lstNoPk));
			template.Replace("{{SqlParameters}}", FormatSqlParameterList(syntax, lstNoPk));
			template.Replace("{{SetProperties}}", FormatSetProperties(instructions.Properties));

			return GetFormattedCSharpResult($"{instructions.SubjectName}Repository.cs", template);
		}

		private string FormatSelectList(IList<ClassMemberStrings> properties, string prefix = null)
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
				p => $"                {p.ColumnName} = @{p.Property}",
				"," + Environment.NewLine);

			return content;
		}

		private string FormatSqlParameterList(BaseSqlEngineSyntax syntax, IList<ClassMemberStrings> properties)
		{
			var content = GetTextBlock(
				properties,
				p => $@"{syntax.FormatSqlParameter(p)}
									
			lst.Add(p);",
				Environment.NewLine);

			return content;
		}
		
		private string FormatSetProperties(IList<ClassMemberStrings> properties)
		{
			var content = GetTextBlock(
				properties,
				p => $"            {FormatSetProperty(p)}",
				Environment.NewLine);

			return content;
		}

		private static string FormatSetProperty(ClassMemberStrings p)
		{
			//Examples
			//r["IntValue"] = Convert.ToInt32(r["IntValue"];
			//r["NullableIntValue"] == DBNull.Value ? null : (int?)Convert.ToInt32(r["NullableIntValue"]);
			//r["GuidValue"] = Guid.Parse(Convert.ToString(r["GuidValue")];
			//r["NullableGuidValue"] == DBNull.Value ? null : (Guid?)Guid.Parse(Convert.ToString(r["NullableGuidValue"]));

			var dr = $"r[\"{p.ColumnName}\"]";

			var method = string.Format(p.ConversionMethodSignature, dr);

			var content = $"e.{p.Property} = ";

			if (p.IsDbNullable)

				//The Alias already has the question mark suffix if nullable
				content += $"{dr} == DBNull.Value ? null : ({p.SystemTypeAlias})";

			content += $"{method};";

			return content;
		}

		private static string FormatInsertExecution(ClassMemberStrings primaryKey)
		{
			string content;

			if (primaryKey.IsIdentity)
			{
				var method = string.Format(primaryKey.ConversionMethodSignature, "GetScalar(dr, \"PK\")");

				content = $@"
			using (var dr = ExecuteReaderText(sql, lst.ToArray()))
			{{
				return {method};
			}}";
			}
			else
			{
				content = $@"
			ExecuteNonQuery(sql, lst.ToArray());

			return entity.{primaryKey.Property};";
			}

			return content;
		}
	}
}
