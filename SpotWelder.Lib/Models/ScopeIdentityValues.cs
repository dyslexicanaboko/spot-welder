namespace SpotWelder.Lib.Models;

public class ScopeIdentityValues
{
  public required string ScopeIdentity { get; set; }
  
  public required string PrimaryKeyColumnName { get; set; }
  
  public required string PrimaryKeyDefault { get; set; }

  public static ScopeIdentityValues Empty()
    => new()
    {
      ScopeIdentity = string.Empty,
      PrimaryKeyColumnName = string.Empty,
      PrimaryKeyDefault = string.Empty
    };
}
