using System;

namespace SpotWelder.Ui.Models;

/// <summary>
/// Used to route errors from a child window/control to a parent window.
/// </summary>
/// <param name="exception"></param>
/// <param name="message"></param>
public class ChildErrorEventArgs(Exception exception, string message = "") : EventArgs
{
  public Exception Exception { get; } = exception;

  public string Message { get; } = message;
}
