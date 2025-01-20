<Query Kind="Program">
  <NuGetReference>Npgsql</NuGetReference>
  <Namespace>Npgsql</Namespace>
  <Namespace>NpgsqlTypes</Namespace>
</Query>

const string ConnectionString = "Host=localhost;Username=postgres;Password=postgres;Database=scratchspace";

void Main()
{
	//GetFullSchemaInfo("public.types_table");
	GetFullSchemaInfo("public.types_table_nullable");
}

//Looking over parameter syntax
private void Blah()
{
	using (var con = new NpgsqlConnection(""))
	{
		con.Open();

		using (var cmd = new NpgsqlCommand("", con))
		{
			var p = new NpgsqlParameter();
			p.ParameterName = "";
			p.NpgsqlDbType = NpgsqlDbType.Bit;
			p.Value = null;
			p.Scale = 0;
			p.Size = 0;
			p.Precision = 0;
			
			using (var dr = cmd.ExecuteReader())
			{
				while (dr.Read())
				{
					//var obj = Convert.ToInt32(dr[""]);
				}
			}
		}
	}
}

private void GetFullSchemaInfo(string target)
{
	using (var con = new NpgsqlConnection(ConnectionString))
	{
		using (var cmd = new NpgsqlCommand($"SELECT * FROM {target} LIMIT 0", con))
		{
			con.Open();

			using (var dr = cmd.ExecuteReader())
			{
				using (var tblSchema = dr.GetSchemaTable())
				{
					Console.WriteLine(tblSchema.Rows.Count);

					foreach (DataRow row in tblSchema.Rows)
					{
						var column = row.Field<string>("ColumnName");
						var sqlType = row.Field<string>("DataTypeName");
						var size = row.Field<int>("ColumnSize");
						var precision = row.Field<int>("NumericPrecision");
						var scale = row.Field<int>("NumericScale");

						//row.Dump();
						
						Console.WriteLine($"Column: {column} SqlType: {sqlType} Precision: {precision} Scale: {scale} Size: {size}");
					}
				}
			}

			using (var da = new NpgsqlDataAdapter(cmd))
			{
				var dt = new DataTable();

				da.FillSchema(dt, SchemaType.Source);

				if (dt.PrimaryKey.Any())
				{
					var pk = dt.PrimaryKey[0];

					Console.WriteLine($"\nPrimaryKey Name: {pk.ColumnName} SystemType: {pk.DataType.Name}\n");
				}

				Console.WriteLine(dt.Columns.Count);

				foreach (DataColumn dc in dt.Columns)
				{
					Console.WriteLine($"Column: {dc.ColumnName} IsNullable: {dc.AllowDBNull} SystemType: {dc.DataType.Name}");
				}
			}
		}
	}
}


