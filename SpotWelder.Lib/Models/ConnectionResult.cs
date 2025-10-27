using System;

namespace SpotWelder.Lib.Models
{
  public class ConnectionResult
  {
    public bool Success { get; set; }

    public string Message => ReturnedException == null ? string.Empty : ReturnedException.Message;

    public Exception? ReturnedException { get; set; }
  }
}
