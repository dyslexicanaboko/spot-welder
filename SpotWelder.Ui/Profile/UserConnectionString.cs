﻿using SpotWelder.Lib;

namespace SpotWelder.Ui.Profile
{
  public class UserConnectionString
  {
    public SqlEngine SqlEngine { get; set; }

    public string ConnectionString { get; set; } = string.Empty;

    public bool Verified { get; set; }

    public override string ToString() => ConnectionString;
  }
}
