<Query Kind="Program">
  <NuGetReference>Microsoft.Data.SqlClient</NuGetReference>
  <Namespace>Microsoft.Data.SqlClient</Namespace>
</Query>

const string ConnectionString = "Data Source=.;Database=ScratchSpace;Integrated Security=SSPI;Encrypt=False";

void Main()
{
	//GetFullSchemaInfo("dbo.TypesTable");
	GetFullSchemaInfo("dbo.TypesTableNullable");
}

private void Attempt1()
{
	using (var con = new SqlConnection(ConnectionString))
	{
		con.Open();

		DataTable dtResult = new DataTable();

		using (SqlCommand command = con.CreateCommand())
		{
			command.CommandText = String.Format("SELECT TOP 0 * FROM dbo.TypesTable");
			command.CommandType = CommandType.Text;

			SqlDataReader reader = command.ExecuteReader(CommandBehavior.SchemaOnly);

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
	using (var con = new SqlConnection(ConnectionString))
	{
		con.Open();

		using (var cmd = new SqlCommand("SET FMTONLY ON; SELECT * FROM dbo.TypesTable; SET FMTONLY OFF;", con))
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
private void GetFullSchemaInfo(string target)
{
	using (var con = new SqlConnection(ConnectionString))
	{
		using (var cmd = new SqlCommand($"SET FMTONLY ON; SELECT TOP 0 * FROM {target}; SET FMTONLY OFF;", con))
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
						var precision = row.Field<short>("NumericPrecision");
						var scale = row.Field<short>("NumericScale");

						Console.WriteLine($"Column: {column} SqlType: {sqlType} Precision: {precision} Scale: {scale} Size: {size}");
					}
				}
			}

			using (var da = new SqlDataAdapter(cmd))
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


