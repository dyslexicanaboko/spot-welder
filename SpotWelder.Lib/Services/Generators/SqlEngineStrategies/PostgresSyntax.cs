using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.CodeFactory;
using System;
using System.Collections.Generic;
using System.Data;

namespace SpotWelder.Lib.Services.Generators.SqlEngineStrategies
{
	public class PostgresSyntax
		: BaseSqlEngineSyntax
	{
		public PostgresSyntax(SqlEngine sqlEngine) 
			: base(sqlEngine)
		{
			
		}

		/// <inheritdoc />
		public override IList<string> SqlNamespaces { get; protected set; } = new List<string>
		{
			"Npgsql",
			"NpgsqlTypes"
		};

		/// <inheritdoc />
		public override string ConnectionObject { get; protected set; } = "NpgsqlConnection";

		/// <inheritdoc />
		public override string ParameterObject { get; protected set; } = "NpgsqlParameter";

		/// <inheritdoc />
		public override string ParameterDbTypeProperty { get; protected set; } = "NpgsqlDbType";

		/// <inheritdoc />
		public override string ParameterDbTypeEnum { get; protected set; } = "NpgsqlDbType";

    /// <inheritdoc />
    public override ScopeIdentityValues GetScopeIdentity(string pkColumnName) => new()
    {
      PrimaryKeyColumnName = $"{pkColumnName},",
      PrimaryKeyDefault = "DEFAULT,",
      ScopeIdentity = $"""
			RETURNING {pkColumnName} AS PK;
			"""
    };

    /// <inheritdoc />
    public override string FormatSqlParameter(ClassMemberStrings properties)
		{
			//The generic DBType cannot be used here because it's SQL Engine specific
			var t = properties.DatabaseType;
			var sqlDataType = SqlEngineTypes.GetDbTypeAsSqlDataTypeEnum(t);

			var valueContent = properties.IsDbNullable ?
				$"entity.{properties.Property} == null ? DBNull.Value : entity.{properties.Property}" :
				$"entity.{properties.Property}";

			var content =
				$@"            p = new NpgsqlParameter();
			p.ParameterName = ""@{properties.ColumnName}"";
			p.NpgsqlDbType = NpgsqlDbType.{sqlDataType};
			p.Value = {valueContent};";

			//TODO: Need to work through every type to see what the combinations are
			if (t == DbType.DateTime2) content += Environment.NewLine + $"            p.Scale = {properties.Scale};";

			if (t == DbType.Decimal)
				content += Environment.NewLine +
					$@"            p.Scale = {properties.Scale};
			p.Precision = {properties.Precision};";

			if (t is DbType.AnsiString or DbType.String or DbType.AnsiStringFixedLength or DbType.StringFixedLength)
				content += Environment.NewLine + $"            p.Size = {properties.Size};";

			return content;
		}
	}
}
