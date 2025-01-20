using SpotWelder.Lib.Services.CodeFactory;
using System;
using System.Data;

namespace SpotWelder.Lib.Services.Generators.SqlEngineStrategies
{
	public class SqlServerSyntax
		: BaseSqlEngineSyntax
	{
		public SqlServerSyntax(SqlEngine sqlEngine) 
			: base(sqlEngine)
		{
			
		}

		public override string FormatSqlParameter(ClassMemberStrings properties)
		{
			//The generic DBType cannot be used here because it's SQL Engine specific
			var t = properties.DatabaseType;
			var sqlDataType = SqlEngineTypes.GetDbTypeAsSqlDataTypeEnum(t);


			var content =
				$@"            p = new SqlParameter();
			p.ParameterName = ""@{properties.Property}"";
			p.SqlDbType = SqlDbType.{sqlDataType};
			p.Value = entity.{properties.Property};";

			//TODO: Need to work through every type to see what the combinations are
			if (t == DbType.DateTime2) content += Environment.NewLine + $"            p.Scale = {properties.Scale};";

			if (t == DbType.Decimal)
				content += Environment.NewLine +
					$@"            p.Scale = {properties.Scale};
			p.Precision = {properties.Precision};";

			if (t is DbType.AnsiString or DbType.String or DbType.AnsiStringFixedLength or DbType.StringFixedLength)
				content += Environment.NewLine + $"            p.Size = {properties.Size}";

			return content;
		}
	}
}
