using SpotWelder.Lib;

namespace SpotWelder.Ui.ViewModels
{
  public class ConnectionStringViewModel
  {
    public SqlEngine SqlEngine { get; set; } = SqlEngine.SqlServer;

    public string ServerName { get; set; } = string.Empty;

    public string DatabaseName { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public bool IsIntegratedSecurity { get; set; }

    public bool IsEncrypted { get; set; }

    public string ConnectionString { get; set; } = string.Empty;
  }
}
