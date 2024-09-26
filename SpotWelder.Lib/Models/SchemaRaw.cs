﻿using System.Data;

namespace SpotWelder.Lib.Models
{
  public class SchemaRaw
  {
    public DataTable GenericSchema { get; set; }
    public DataTable SqlServerSchema { get; set; }

    public SchemaRaw(DataTable genericSchema, DataTable sqlServerSchema)
    {
      GenericSchema = genericSchema;
      SqlServerSchema = sqlServerSchema;
    }
  }
}
