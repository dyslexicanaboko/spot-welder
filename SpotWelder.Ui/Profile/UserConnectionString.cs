using SpotWelder.Lib;
using System;

namespace SpotWelder.Ui.Profile
{
  public class UserConnectionString : IEquatable<UserConnectionString>
  {
    public SqlEngine SqlEngine { get; set; }

    public string ConnectionString { get; set; } = string.Empty;

    public bool Verified { get; set; }

    public override string ToString() => ConnectionString;

    public bool Equals(UserConnectionString? other)
    {
      if (other == null)
        return false;

      return ConnectionString == other.ConnectionString;
    }

    public override bool Equals(object? obj)
    {
      if (obj == null || GetType() != obj.GetType())
        return false;

      return Equals(obj as UserConnectionString);
    }

    public override int GetHashCode()
    {
      return ConnectionString.GetHashCode();
    }

    public static bool operator ==(UserConnectionString? lhs, UserConnectionString? rhs)
    {
      if (lhs is null)
      {
        if (rhs is null)
        {
          return true;
        }

        return false;
      }

      return lhs.Equals(rhs);
    }

    public static bool operator !=(UserConnectionString? lhs, UserConnectionString? rhs) => !(lhs == rhs);
  }
}
