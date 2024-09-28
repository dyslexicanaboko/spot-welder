namespace SpotWelder.Lib.Models
{
  public class ConnectionStringMeta
  {
    public string ServerName { get; set; } = string.Empty;

    public string DatabaseName { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public bool IsIntegratedSecurity { get; set; }

    public bool IsEncrypted { get; set; }
  }
}
