<Query Kind="Program">
  <NuGetReference>Npgsql</NuGetReference>
  <Namespace>Npgsql</Namespace>
</Query>

const string ConnectionString = "Host=localhost;Username=postgres;Password=postgres;Database=scratchspace";

void Main()
{
	//Attempt1();
	//Attempt2();
	GetFullSchemaInfo();
}

private void Attempt1()
{
	using (var con = new NpgsqlConnection(ConnectionString))
	{
		con.Open();

		DataTable dtResult = new DataTable();

		using (var command = con.CreateCommand())
		{
			command.CommandText = String.Format("SELECT * FROM public.types_table LIMIT 0");
			command.CommandType = CommandType.Text;

			var reader = command.ExecuteReader(CommandBehavior.SchemaOnly);

			dtResult.Load(reader);

			foreach (DataColumn dc in dtResult.Columns)
			{
				dc.Dump();
			}
		}
	}
}

private void Attempt2()
{
	using (var con = new NpgsqlConnection(ConnectionString))
	{
		con.Open();

		//There is no exact equivalent for Postgres - this query does not work.
		using (var cmd = new NpgsqlCommand("SET FMTONLY ON; SELECT * FROM public.types_table LIMIT 0; SET FMTONLY OFF;", con))
		{
			using (var dr = cmd.ExecuteReader())
			{
				while (dr.Read())
				{
					for (int i = 0; i < dr.FieldCount; i++)
					{
						//Console.WriteLine(dr.GetFieldType(i));
						dr.GetProviderSpecificFieldType(i).Dump(); //DataType
																   //Console.WriteLine(dr.GetDataTypeName(i));
					}
				}
			}
		}


	}
}

//https://stackoverflow.com/questions/24164439/how-to-get-the-exact-type-of-numeric-columns-incl-scale-and-precision
private void GetFullSchemaInfo()
{
	using (var con = new NpgsqlConnection(ConnectionString))
	{
		using (var cmd = new NpgsqlCommand("SELECT * FROM public.types_table LIMIT 0", con))
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


